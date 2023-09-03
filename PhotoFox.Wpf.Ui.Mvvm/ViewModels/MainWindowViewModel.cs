using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NLog;
using PhotoFox.Model;
using PhotoFox.Services;
using PhotoFox.Storage.Blob;
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
        IRecipient<UnloadVideoMessage>,
        IRecipient<UnloadAlbumMessage>,
        IRecipient<UpdateStatusMessage>,
        IRecipient<SetStatusMessage>,
        IRecipient<RefreshVisiblePhotosMessage>
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoService photoService;

        private readonly IVideoService videoService;

        private readonly IPhotoAlbumService photoAlbumService;

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly IMessenger messenger;

        private readonly IContext context;

        private string loadingStatusText = string.Empty;

        private FolderViewModel? selectedFolder;

        private PhotoViewModel? selectedPhoto;

        private VideoViewModel? selectedVideo;

        private AlbumViewModel? selectedAlbum;

        private CancellationTokenSource cancellationTokenSource;

        private bool isLoadingAlbums;

        private bool isLoadingPhotos;

        private int minimumRating;

        public MainWindowViewModel(
            IPhotoService photoService,
            IVideoService videoService,
            IPhotoAlbumService photoAlbumService,
            IPhotoFileStorage photoFileStorage,
            IMessenger messenger,
            IContext context,
            AddPhotosCommand addPhotosCommand,
            OpenGpsLocationCommand openGpsLocationCommand,
            DeletePhotoCommand deletePhotoCommand,
            DeleteVideoCommand deleteVideoCommand,
            AddAlbumCommand addAlbumCommand,
            DeleteAlbumCommand deleteAlbumCommand,
            SaveChangesCommand saveChangesCommand,
            SetPermissionsCommand setPermissionsCommand,
            EditSelectedAlbumCommand editSelectedAlbumCommand,
            DecrementRatingCommand decrementRatingCommand,
            IncrementRatingCommand incrementRatingCommand,
            SetRatingCommand setRatingCommand)
        {
            this.photoService = photoService;
            this.videoService = videoService;
            this.photoAlbumService = photoAlbumService;
            this.photoFileStorage = photoFileStorage;
            this.messenger = messenger;
            this.context = context;

            this.Folders = new ObservableCollection<FolderViewModel>();
            this.Albums = new ObservableCollection<AlbumViewModel>();
            this.Photos = new ObservableCollection<PhotoViewModel>();
            this.Videos = new ObservableCollection<VideoViewModel>();
            this.VisiblePhotos = new ObservableCollection<PhotoViewModel>();

            AddPhotosCommand = addPhotosCommand;
            OpenGpsLink = openGpsLocationCommand;
            DeletePhotoCommand = deletePhotoCommand;
            DeleteVideoCommand = deleteVideoCommand;
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
            EditSelectedAlbumCommand = editSelectedAlbumCommand;
            DecrementRatingCommand = decrementRatingCommand;
            IncrementRatingCommand = incrementRatingCommand;
            SetRatingCommand = setRatingCommand;

            messenger.Register<RefreshAlbumsMessage>(this);
            messenger.Register<LoadPhotoMessage>(this);
            messenger.Register<UnloadPhotoMessage>(this);
            messenger.Register<UpdateStatusMessage>(this);
            messenger.Register<SetStatusMessage>(this);
            messenger.Register<UnloadVideoMessage>(this);
            messenger.Register<UnloadAlbumMessage>(this);
            messenger.Register<RefreshVisiblePhotosMessage>(this);

            this.PropertyChanged += MainWindowViewModel_PropertyChanged;

            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task ItemChanged()
        {
            await OnSelectedAlbumChanged();
        }

        private async void MainWindowViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.SelectedFolder))
            {
                OnSelectedFolderChanged();
            }

            if (e.PropertyName == nameof(this.SelectedAlbum))
            {
                await OnSelectedAlbumChanged();
            }
        }

        private void OnSelectedFolderChanged()
        {
            this.Albums.Clear();

            if (this.SelectedFolder == null)
            {
                return;
            }

            foreach (var album in Cache.Albums.Where(a => a.Folder == this.SelectedFolder.Title).OrderBy(o => o.SortOrder))
            {
                if (album.Image == null)
                {
                    string coverId = album.CoverPhotoId;
                    if (string.IsNullOrEmpty(coverId))
                    {
                        album.Image = new BitmapImage(new Uri("pack://application:,,,/Images/placeholder.jpg"));
                    }
                    else
                    {
                        _ = Task.Run(() => LoadThumbnail(coverId, album));
                    }
                }

                this.Albums.Add(album);
            }
        }

        private async Task OnSelectedAlbumChanged()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            await LoadAllItems(cancellationTokenSource.Token);
        }

        private async Task LoadAllItems(CancellationToken cancel)
        {
            IsLoadingPhotos = true;

            await Task.WhenAll(
                LoadPhotos(cancel),
                LoadVideos(cancel));

            IsLoadingPhotos = false;
        }

        public int MinimumRating
        {
            get => this.minimumRating;
            set
            {
                if (value != this.minimumRating)
                {
                    this.minimumRating = value;
                    this.OnPropertyChanged(nameof(MinimumRating));
                    RefreshVisiblePhotos();
                }
            }
        }

        public ObservableCollection<FolderViewModel> Folders { get; }

        public ObservableCollection<AlbumViewModel> Albums { get; }

        public ObservableCollection<PhotoViewModel> Photos { get; }

        public ObservableCollection<VideoViewModel> Videos { get; set; }

        public ObservableCollection<PhotoViewModel> VisiblePhotos { get; set; }

        public int AlbumCount => Cache.Albums.Count;

        public ICommand AddPhotosCommand { get; }
        public ICommand OpenGpsLink { get; }
        public ICommand DeletePhotoCommand { get; }
        public ICommand DeleteVideoCommand { get; }
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
        public ICommand EditSelectedAlbumCommand { get; }
        public ICommand DecrementRatingCommand { get; }
        public ICommand IncrementRatingCommand { get; }
        public ICommand SetRatingCommand { get; }

        public bool LoadingAlbumsIndicatorVisible => isLoadingAlbums;

        public bool LoadingPhotosIndicatorVisible => isLoadingPhotos;

        public bool FolderListVisible => !isLoadingAlbums;

        public bool PhotosListVisible => !isLoadingPhotos;

        public bool IsLoadingAlbums
        {
            get => isLoadingAlbums;
            set
            {
                isLoadingAlbums = value;
                OnPropertyChanged(nameof(FolderListVisible));
                OnPropertyChanged(nameof(LoadingAlbumsIndicatorVisible));
            }
        }

        public bool IsLoadingPhotos
        {
            get => isLoadingPhotos;
            set
            {
                isLoadingPhotos = value;
                OnPropertyChanged(nameof(LoadingPhotosIndicatorVisible));
                OnPropertyChanged(nameof(PhotosListVisible));
            }
        }

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

        public VideoViewModel? SelectedVideo
        {
            get => selectedVideo;
            set
            {
                if (ReferenceEquals(this.selectedVideo, value))
                {
                    return;
                }

                selectedVideo = value;
                OnPropertyChanged(nameof(SelectedVideo));
            }
        }

        public IEnumerable<PhotoViewModel> SelectedPhotos
        {
            get => this.Photos.Where(p => p.IsSelected);
        }

        public IEnumerable<VideoViewModel> SelectedVideos
        {
            get => this.Videos.Where(p => p.IsSelected);
        }

        public FolderViewModel? SelectedFolder
        {
            get => this.selectedFolder;
            set
            {
                if (ReferenceEquals(this.selectedFolder, value))
                {
                    return;
                }

                selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
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
            await LoadAllItems(cancellationTokenSource.Token);
        }

        private void LoadPhoto(Photo photo, CancellationToken token)
        {
            if (this.Photos.Any(p => p.Item.PhotoId == photo.PhotoId))
            {
                this.Photos.Remove(this.Photos.First(p => p.Item.PhotoId == photo.PhotoId));
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            var viewModel = new PhotoViewModel(photo);

            Log.Trace($"Adding {photo.PhotoId}");

            _ = Task.Run(() => LoadThumbnail(photo.PhotoId, viewModel), token);

            this.Photos.Add(viewModel);
            this.OnPropertyChanged(nameof(this.Photos.Count));
        }

        private void LoadVideo(Video video, CancellationToken token)
        {
            if (this.Videos.Any(p => p.Item.VideoId == video.VideoId))
            {
                this.Videos.Remove(this.Videos.First(p => p.Item.VideoId == video.VideoId));
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            var viewModel = new VideoViewModel(video);

            Log.Trace($"Adding video {video.VideoId}");

            _ = Task.Run(() => LoadThumbnail(video.VideoId, viewModel), token);

            this.Videos.Add(viewModel);
            this.OnPropertyChanged(nameof(this.Videos.Count));
        }

        public async Task LoadThumbnail(string thumbnailId, IHasThumbnail viewModel)
        {
            var blob = await this.photoFileStorage.GetThumbnailAsync(thumbnailId);
            if (blob == null)
            {
                context.BeginInvoke(() => viewModel.Image = new BitmapImage(new Uri("pack://application:,,,/Images/error.jpg")));
                return;
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
                await foreach (var photo in this.photoService.GetPhotosInAlbumAsync(this.SelectedAlbum.AlbumId))
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    LoadPhoto(photo, token);
                }
            }

            RefreshVisiblePhotos();
        }

        private void RefreshVisiblePhotos()
        {
            this.VisiblePhotos.Clear();
            foreach (var photo in this.Photos.Where(p => p.StarRating >= this.MinimumRating))
            {
                this.VisiblePhotos.Add(photo);
            }
        }

        public async Task LoadVideos(CancellationToken token)
        {
            this.Videos.Clear();
            this.OnPropertyChanged(nameof(this.Videos.Count));

            if (this.SelectedAlbum != null && !string.IsNullOrEmpty(this.SelectedAlbum.AlbumId))
            {
                await foreach (var video in this.videoService.GetVideosInAlbumAsync(this.SelectedAlbum.AlbumId))
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    LoadVideo(video, token);
                }
            }
        }

        private async Task LoadAlbums()
        {
            Cache.Albums.Clear();
            this.Folders.Clear();

            IsLoadingAlbums = true;

            await foreach (var album in this.photoAlbumService.GetAllAlbumsAsync())
            {
                if (!this.Folders.Any(f => f.Title == album.Folder))
                {
                    this.Folders.Add(new FolderViewModel(album.Folder));
                }

                var viewModel = new AlbumViewModel
                {
                    Title = album.Title,
                    AlbumId = album.AlbumId,
                    Folder = album.Folder,
                    Description = album.Description,
                    SortOrder = album.SortOrder,
                    CoverPhotoId = album.CoverPhotoId,
                };

                Cache.Albums.Add(viewModel);
                this.OnPropertyChanged(nameof(this.AlbumCount));
            }

            foreach (var folderViewModel in Folders)
            {
                foreach (var albumViewModel in Cache.Albums.Where(a => a.Folder == folderViewModel.Title))
                {
                    folderViewModel.Albums.Add(albumViewModel);
                }
            }

            IsLoadingAlbums = false;
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
            this.messenger.Send(new OpenPhotoMessage(selected.Item.PhotoId));
        }

        public void OpenSelectedVideo()
        {
            if (this.Videos.Count(p => p.IsSelected) != 1)
            {
                return;
            }

            var selected = this.Videos.Single(p => p.IsSelected);
            this.messenger.Send(new OpenVideoMessage(selected.Item.VideoId, selected.Item.FileExt));
        }

        public bool OpenSelectedImageCanExecute()
        {
            return this.Photos.Count(p => p.IsSelected) == 1;
        }

        private void SetAlbumCoverCommandExecute()
        {
            if (this.SelectedPhoto == null 
                || this.SelectedAlbum?.AlbumId == null 
                || this.SelectedPhoto.Image == null)
            {
                return;
            }

            this.photoAlbumService.SetCoverImageAsync(this.SelectedAlbum.AlbumId, this.SelectedPhoto.Item.PhotoId);
            this.SelectedAlbum.Image = this.SelectedPhoto.Image;
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

            if (albumId != string.Empty || string.IsNullOrEmpty(response.NewAlbumName))
            {
                return albumId;
            }

            var album = new PhotoAlbum(
                Guid.NewGuid().ToString(), 
                response.NewAlbumName, 
                string.Empty,
                this.Photos.First(p => p.IsSelected).Item.PhotoId,
                string.Empty,
                string.Empty,
                false);

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

            foreach (var photo in this.Photos.Where(p => p.IsSelected).Select(p => p.Item))
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
                await this.photoAlbumService.RemoveFromAlbumAsync(this.SelectedAlbum.AlbumId, photo.Item.PhotoId);
                await this.photoAlbumService.AddPhotoToAlbumAsync(albumId, photo.Item.PhotoId, photo.Item.DateTaken);
                toRemove.Add(photo);
            }

            toRemove.ForEach(p => 
                {
                    this.Photos.Remove(p);
                    this.VisiblePhotos.Remove(p);
                }
            );
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
            this.VisiblePhotos.Remove(message.ViewModel);
            this.OnPropertyChanged(nameof(this.Photos.Count));
        }

        public void Receive(UnloadVideoMessage message)
        {
            this.Videos.Remove(message.ViewModel);
            this.OnPropertyChanged(nameof(this.Videos.Count));
        }

        public void Receive(UnloadAlbumMessage message)
        {
            this.Albums.Remove(message.ViewModel);
            this.OnPropertyChanged(nameof(this.Albums.Count));
        }

        public async void Receive(RefreshAlbumsMessage message)
        {
            await this.LoadAlbums();
        }

        public void Receive(RefreshVisiblePhotosMessage message)
        {
            this.RefreshVisiblePhotos();
        }

        public void Receive(UpdateStatusMessage message)
        {
            this.LoadingStatusText = message.Message;
        }

        public async void ReloadExifExecute()
        {
            var photosToProcess = new List<PhotoViewModel>();
            photosToProcess.AddRange(this.SelectedPhotos);

            foreach (var photo in photosToProcess.Select(p => p.Item))
            {
                var newPhoto = await this.photoService.ReloadExifDataAsync(photo.DateTaken, photo.PhotoId);

                this.LoadPhoto(newPhoto, cancellationTokenSource.Token);
            }
        }
    }
}
