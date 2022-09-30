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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class MainWindowViewModel : ObservableObject,
        IRecipient<RefreshAlbumsMessage>,
        IRecipient<LoadPhotoMessage>,
        IRecipient<UnloadPhotoMessage>,
        IRecipient<UpdateStatusMessage>
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoService photoService;

        private readonly IPhotoAlbumService photoAlbumService;

        private readonly IPhotoFileStorage photoFileStorage;

        private DateTime batchId = DateTime.MinValue;

        private bool isLoading = false;

        private string loadingStatusText = string.Empty;

        private PhotoViewModel? selectedPhoto;

        private AlbumViewModel? selectedAlbum;

        private CancellationTokenSource cancellationTokenSource;

        public MainWindowViewModel(
            IPhotoService photoService,
            IPhotoAlbumService photoAlbumService,
            IPhotoFileStorage photoFileStorage,
            IMessenger messenger,
            AddPhotosCommand addPhotosCommand,
            OpenGpsLocationCommand openGpsLocationCommand,
            DeletePhotoCommand deletePhotoCommand,
            AddAlbumCommand addAlbumCommand,
            DeleteAlbumCommand deleteAlbumCommand,
            SaveChangesCommand saveChangesCommand)
        {
            this.photoService = photoService;
            this.photoAlbumService = photoAlbumService;
            this.photoFileStorage = photoFileStorage;

            this.Albums = new ObservableCollection<AlbumViewModel>();
            this.Photos = new ObservableCollection<PhotoViewModel>();

            AddPhotosCommand = addPhotosCommand;
            OpenGpsLink = openGpsLocationCommand;
            DeletePhotoCommand = deletePhotoCommand;
            AddAlbumCommand = addAlbumCommand;
            DeleteAlbumCommand = deleteAlbumCommand;
            StopLoadingCommand = new RelayCommand(StopLoadingExecute, StopLoadingCanExecute);
            SaveChangesCommand = saveChangesCommand;

            messenger.Register<RefreshAlbumsMessage>(this);
            messenger.Register<LoadPhotoMessage>(this);
            messenger.Register<UnloadPhotoMessage>(this);
            messenger.Register<UpdateStatusMessage>(this);
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
                cancellationTokenSource.Cancel();
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
            this.batchId = DateTime.Now.Date;

            Log.Debug($"Initial load. Batch ID is {batchId}");

            cancellationTokenSource = new CancellationTokenSource();

            await Task.WhenAll(
                LoadAlbums(),
                LoadPhotos(20, cancellationTokenSource.Token)
            );
        }

        private async Task LoadPhoto(Photo photo)
        {
            if (this.Photos.Any(p => p.Photo.PhotoId == photo.PhotoId))
            {
                this.Photos.Remove(this.Photos.First(p => p.Photo.PhotoId == photo.PhotoId));
            }

            var blob = await this.photoFileStorage.GetThumbnailAsync(photo.PhotoId);
            if (blob == null)
            {
                throw new InvalidOperationException($"Unable to find {photo.PhotoId} in thumbnail storage");
            }

            var thumbnail = GetImageFromBytes(blob.ToArray());

            var viewModel = new PhotoViewModel(thumbnail, photo);

            Log.Trace($"Adding {photo.PhotoId}");

            this.Photos.Add(viewModel);
        }

        public async Task LoadPhotosInSelectedAlbum(CancellationToken token)
        {
            if (isLoading || this.SelectedAlbum == null)
            {
                return;
            }

            this.Photos.Clear();

            isLoading = true;
            await foreach (var photo in this.photoAlbumService.GetPhotosInAlbum(this.SelectedAlbum.AlbumId))
            {
                await LoadPhoto(photo);
            }
        }

        public async Task LoadPhotos(int minLoadCount, CancellationToken token)
        {
            if (isLoading 
                || batchId == DateTime.MinValue
                || token.IsCancellationRequested)
            {
                return;
            }

            isLoading = true;
            var numPhotos = 0;

            while (numPhotos < minLoadCount)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                Log.Trace($"Loading photos. Batch ID is {batchId}");
                string strDate = batchId.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
                this.LoadingStatusText = $"Loading photos from {strDate}...";

                await foreach (var photo in this.photoService.GetPhotosByDateTaken(this.batchId))
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    await LoadPhoto(photo);

                    numPhotos++;
                }

                batchId = batchId.AddDays(-1);
                if (batchId < DateTime.UtcNow.AddMonths(-12))
                {
                    break;
                }
            }

            isLoading = false;

            LoadingStatusText = "Loading complete.";
        }

        private async Task LoadAlbums()
        {
            this.Albums.Clear();

            await foreach (var album in this.photoAlbumService.GetAllAlbums())
            {
                var viewModel = new AlbumViewModel
                {
                    Title = album.Title,
                    AlbumId = album.AlbumId
                };

                string coverId = album.CoverPhotoId;

                var blob = await this.photoFileStorage.GetThumbnailAsync(coverId);
                viewModel.Image = GetImageFromBytes(blob.ToArray());

                this.Albums.Add(viewModel);
            }
        }

        private BitmapImage GetImageFromBytes(byte[] bytes)
        {
            var stream = new MemoryStream();

            Image img;
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            img = Image.FromStream(stream);

            var bitImage = new BitmapImage();
            bitImage.BeginInit();

            var ms = new MemoryStream();

            img.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            bitImage.StreamSource = ms;
            bitImage.EndInit();

            return bitImage;
        }

        public async void Receive(RefreshAlbumsMessage message)
        {
            await this.LoadAlbums();
        }

        public async Task LoadMore()
        {
            if (this.SelectedAlbum != null || cancellationTokenSource == null)
            {
                return;
            }

            await LoadPhotos(20, cancellationTokenSource.Token);
        }

        private void StopLoadingExecute()
        {
            cancellationTokenSource.Cancel();
        }

        private bool StopLoadingCanExecute()
        {
            return true;
        }

        public async void Receive(LoadPhotoMessage message)
        {
            await LoadPhoto(message.Photo);
        }

        public void Receive(UnloadPhotoMessage message)
        {
            this.Photos.Remove(message.ViewModel);
        }

        public void Receive(UpdateStatusMessage message)
        {
            this.LoadingStatusText = message.Message;
        }
    }
}
