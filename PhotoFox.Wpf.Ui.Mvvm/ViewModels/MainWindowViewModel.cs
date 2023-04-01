using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NLog;
using PhotoFox.Model;
using PhotoFox.Services;
using PhotoFox.Storage.Blob;
using PhotoFox.Ui.Wpf.Mvvm.ViewModels;
using PhotoFox.Wpf.Ui.Mvvm.Commands;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PhotoAlbum = PhotoFox.Model.PhotoAlbum;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class MainWindowViewModel : ObservableObject,
        IRecipient<RefreshAlbumsMessage>,
        IRecipient<LoadPhotoMessage>,
        IRecipient<UnloadPhotoMessage>,
        IRecipient<UpdateStatusMessage>,
        IRecipient<SetStatusMessage>
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoService photoService;

        private readonly IPhotoAlbumService photoAlbumService;

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly IMessenger messenger;

        private readonly IContext context;

        private string loadingStatusText = string.Empty;

        private PhotoViewModel? selectedPhoto;

        private AlbumViewModel? selectedAlbum;

        private CancellationTokenSource cancellationTokenSource;

        public MainWindowViewModel(
            IPhotoService photoService,
            IPhotoAlbumService photoAlbumService,
            IPhotoFileStorage photoFileStorage,
            IMessenger messenger,
            IContext context,
            AddPhotosCommand addPhotosCommand,
            OpenGpsLocationCommand openGpsLocationCommand,
            DeletePhotoCommand deletePhotoCommand,
            AddAlbumCommand addAlbumCommand,
            DeleteAlbumCommand deleteAlbumCommand,
            SaveChangesCommand saveChangesCommand,
            SetPermissionsCommand setPermissionsCommand)
        {
            this.photoService = photoService;
            this.photoAlbumService = photoAlbumService;
            this.photoFileStorage = photoFileStorage;
            this.messenger = messenger;
            this.context = context;

            this.Albums = new ObservableCollection<AlbumViewModel>();
            this.Photos = new ObservableCollection<PhotoViewModel>();

            AddPhotosCommand = addPhotosCommand;
            OpenGpsLink = openGpsLocationCommand;
            DeletePhotoCommand = deletePhotoCommand;
            AddAlbumCommand = addAlbumCommand;
            DeleteAlbumCommand = deleteAlbumCommand;
            StopLoadingCommand = new RelayCommand(StopLoadingExecute, () => true);
            SaveChangesCommand = saveChangesCommand;
            OpenPhotoCommand = new RelayCommand(OpenSelectedImage, OpenSelectedImageCanExecute);
            AddToAlbumCommand = new RelayCommand(AddToAlbumCommandExecute);
            MoveToAlbumCommand = new RelayCommand(MoveToAlbumCommandExecute);
            SetAlbumCoverCommand = new RelayCommand(SetAlbumCoverCommandExecute, () => this.SelectedPhoto != null);
            ReloadExifCommand = new RelayCommand(ReloadExifExecute, () => this.SelectedPhoto != null);
            SetPermissionsCommand = setPermissionsCommand;

            messenger.Register<RefreshAlbumsMessage>(this);
            messenger.Register<LoadPhotoMessage>(this);
            messenger.Register<UnloadPhotoMessage>(this);
            messenger.Register<UpdateStatusMessage>(this);
            messenger.Register<SetStatusMessage>(this);

            this.PropertyChanged += MainWindowViewModel_PropertyChanged;

            this.cancellationTokenSource = new CancellationTokenSource();
        }

        private async void MainWindowViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.SelectedAlbum))
            {
                await OnSelectedAlbumChanged();
            }
        }

        private async Task OnSelectedAlbumChanged()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            await LoadPhotos(cancellationTokenSource.Token);
        }

        public ObservableCollection<AlbumViewModel> Albums { get; }

        public ObservableCollection<PhotoViewModel> Photos { get; }

        public ICommand AddPhotosCommand { get; }
        public ICommand OpenGpsLink { get; }
        public ICommand DeletePhotoCommand { get; }
        public ICommand AddAlbumCommand { get; }
        public ICommand DeleteAlbumCommand { get; }
        public ICommand StopLoadingCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand OpenPhotoCommand { get; }
        public ICommand AddToAlbumCommand { get; }
        public ICommand MoveToAlbumCommand { get; }
        public ICommand SetAlbumCoverCommand { get; }
        public ICommand ReloadExifCommand { get; }
        public ICommand SetPermissionsCommand { get; }

        public PhotoViewModel? SelectedPhoto
        {
            get => selectedPhoto;
            set
            {
                if (ReferenceEquals(this.selectedPhoto, value))
                {
                    return;
                }

                selectedPhoto = value;
                OnPropertyChanged(nameof(SelectedPhoto));
            }
        }

        public IEnumerable<PhotoViewModel> SelectedPhotos
        {
            get => this.Photos.Where(p => p.IsSelected);
        }

        public AlbumViewModel? SelectedAlbum
        {
            get => selectedAlbum;
            set
            {
                if (ReferenceEquals(this.SelectedAlbum, value))
                {
                    return;
                }

                selectedAlbum = value;
                OnPropertyChanged(nameof(SelectedAlbum));
            }
        }

        public string LoadingStatusText
        {
            get => loadingStatusText;
            set
            {
                loadingStatusText = value;
                OnPropertyChanged(nameof(LoadingStatusText));
            }
        }

        public async Task Load()
        {
            await LoadAlbums();
            await LoadPhotos(cancellationTokenSource.Token);
        }

        private void LoadPhoto(Photo photo, CancellationToken token)
        {
            if (this.Photos.Any(p => p.Photo.PhotoId == photo.PhotoId))
            {
                this.Photos.Remove(this.Photos.First(p => p.Photo.PhotoId == photo.PhotoId));
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            var viewModel = new PhotoViewModel(photo);

            Log.Trace($"Adding {photo.PhotoId}");

            _ = Task.Run(() => LoadPhotoThumbnail(photo, viewModel), token);

            this.Photos.Add(viewModel);
            this.OnPropertyChanged(nameof(this.Photos.Count));
        }

        public async Task LoadPhotoThumbnail(Photo photo, PhotoViewModel viewModel)
        {
            var blob = await this.photoFileStorage.GetThumbnailAsync(photo.PhotoId);
            if (blob == null)
            {
                // TODO: This will be swallowed as it's in Fire and Forget - better to replace with an "Error" image
                throw new InvalidOperationException($"Unable to find {photo.PhotoId} in thumbnail storage");
            }

            var thumbnail = GetImageFromBytes(blob.ToArray());

            context.BeginInvoke(() => viewModel.Image = thumbnail);
        }

        public async Task LoadPhotos(CancellationToken token)
        {
            this.Photos.Clear();
            this.OnPropertyChanged(nameof(this.Photos.Count));

            if (this.SelectedAlbum != null && !string.IsNullOrEmpty(this.SelectedAlbum.AlbumId))
            {
                await LoadPhotosFromAlbum(this.SelectedAlbum.AlbumId, token);
            }
        }

        private async Task LoadPhotosFromAlbum(string albumId, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            await foreach (var photo in this.photoService.GetPhotosInAlbumAsync(albumId))
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                LoadPhoto(photo, token);
            }
        }

        private async Task LoadAlbums()
        {
            this.Albums.Clear();

            await foreach (var album in this.photoAlbumService.GetAllAlbumsAsync())
            {
                var viewModel = new AlbumViewModel
                {
                    Title = album.Title,
                    AlbumId = album.AlbumId
                };

                string coverId = album.CoverPhotoId;

                var blob = await this.photoFileStorage.GetThumbnailAsync(coverId);
                if (blob != null)
                {
                    viewModel.Image = GetImageFromBytes(blob.ToArray());
                }
                else
                {
                    // TODO: Default Cover Image
                }

                this.Albums.Add(viewModel);
                this.OnPropertyChanged(nameof(this.Albums.Count));
            }
        }

        private static BitmapSource GetImageFromBytes(byte[] bytes)
        {
            var stream = new MemoryStream();

            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            using (var img = Image.FromStream(stream))
            {
                var bitImage = new BitmapImage();
                bitImage.BeginInit();

                var ms = new MemoryStream();

                img.Save(ms, ImageFormat.Jpeg);

                ms.Seek(0, SeekOrigin.Begin);
                bitImage.StreamSource = ms;
                bitImage.EndInit();

                bitImage.Freeze();

                return bitImage;
            }
        }

        private void StopLoadingExecute()
        {
            cancellationTokenSource.Cancel();
        }

        public void OpenSelectedImage()
        {
            if (this.Photos.Count(p => p.IsSelected) != 1)
            {
                return;
            }

            var selected = this.Photos.Single(p => p.IsSelected);
            this.messenger.Send(new OpenPhotoMessage(selected.Photo.PhotoId));
        }

        public bool OpenSelectedImageCanExecute()
        {
            return this.Photos.Count(p => p.IsSelected) == 1;
        }

        private void SetAlbumCoverCommandExecute()
        {
            if (this.SelectedPhoto == null 
                || this.SelectedAlbum == null 
                || this.SelectedPhoto.Image == null)
            {
                return;
            }

            this.photoAlbumService.SetCoverImageAsync(this.SelectedAlbum.AlbumId, this.SelectedPhoto.Photo.PhotoId);
            this.SelectedAlbum.SetImage(this.SelectedPhoto.Image);
        }

        private string? GetAlbumIdForMove()
        {
            var selectAlbumMessage = new SelectAlbumMessage();
            SelectAlbumMessageResponse response = this.messenger.Send(selectAlbumMessage);

            if (!response.Result)
            {
                return null;
            }

            var albumId = response.SelectedAlbumId;

            if (albumId != string.Empty)
            {
                return albumId;
            }

            var album = new PhotoAlbum
            {
                AlbumId = Guid.NewGuid().ToString(),
                Description = string.Empty,
                Title = response.NewAlbumName,
                CoverPhotoId = this.Photos.First(p => p.IsSelected).Photo.PhotoId
            };

            this.photoAlbumService.AddAlbumAsync(album);

            return album.AlbumId;
        }

        public async void AddToAlbumCommandExecute()
        {
            var albumId = GetAlbumIdForMove();
            if (albumId is null)
            {
                return;
            }

            foreach (var photo in this.Photos.Where(p => p.IsSelected).Select(p => p.Photo))
            {
                await this.photoAlbumService.AddPhotoToAlbumAsync(albumId, photo.PhotoId, photo.DateTaken);
            }
        }

        public async void MoveToAlbumCommandExecute()
        {
            if (this.SelectedAlbum is null)
            {
                return;
            }

            var albumId = GetAlbumIdForMove();
            if (albumId is null)
            {
                return;
            }

            var toRemove = new List<PhotoViewModel>();

            foreach (var photo in this.Photos.Where(p => p.IsSelected))
            {
                await this.photoAlbumService.RemoveFromAlbumAsync(this.SelectedAlbum.AlbumId, photo.Photo.PhotoId);
                await this.photoAlbumService.AddPhotoToAlbumAsync(albumId, photo.Photo.PhotoId, photo.Photo.DateTaken);
                toRemove.Add(photo);
            }

            toRemove.ForEach(p => this.Photos.Remove(p));
        }

        public void Receive(SetStatusMessage message)
        {
            this.LoadingStatusText = message.Message;
        }

        public void Receive(LoadPhotoMessage message)
        {
            LoadPhoto(message.Photo, cancellationTokenSource.Token);
        }

        public void Receive(UnloadPhotoMessage message)
        {
            this.Photos.Remove(message.ViewModel);
            this.OnPropertyChanged(nameof(this.Photos.Count));
        }

        public async void Receive(RefreshAlbumsMessage message)
        {
            await this.LoadAlbums();
        }

        public void Receive(UpdateStatusMessage message)
        {
            this.LoadingStatusText = message.Message;
        }

        public async void ReloadExifExecute()
        {
            var photosToProcess = new List<PhotoViewModel>();
            photosToProcess.AddRange(this.SelectedPhotos);

            foreach (var photo in photosToProcess.Select(p => p.Photo))
            {
                var newPhoto = await this.photoService.ReloadExifDataAsync(photo.DateTaken, photo.PhotoId);

                this.LoadPhoto(newPhoto, cancellationTokenSource.Token);
            }
        }
    }
}
