using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using NLog;
using PhotoFox.Services;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using PhotoFox.Ui.Wpf.Mvvm.ViewModels;
using PhotoFox.Wpf.Ui.Mvvm.Commands;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoAlbumDataStorage albumStorage;

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly ISettingsStorage settingsStorage;

        private readonly IPhotoMetadataStorage photoMetadataStorage;

        private DateTime batchId = DateTime.MinValue;

        private bool isLoading = false;

        private PhotoViewModel? selectedPhoto;

        public MainWindowViewModel(
            IPhotoAlbumDataStorage photoStorage,
            IPhotoFileStorage photoFileStorage,
            ISettingsStorage settingsStorage,
            IPhotoMetadataStorage photoMetadataStorage,
            AddPhotosCommand addPhotosCommand,
            OpenGpsLocationCommand openGpsLocationCommand,
            DeletePhotoCommand deletePhotoCommand,
            AddAlbumCommand addAlbumCommand)
        {
            this.albumStorage = photoStorage;
            this.photoFileStorage = photoFileStorage;
            this.settingsStorage = settingsStorage;
            this.photoMetadataStorage = photoMetadataStorage;

            this.Albums = new ObservableCollection<AlbumViewModel>();
            this.Photos = new ObservableCollection<PhotoViewModel>();

            AddPhotosCommand = addPhotosCommand;
            OpenGpsLink = openGpsLocationCommand;
            DeletePhotoCommand = deletePhotoCommand;
            AddAlbumCommand = addAlbumCommand;
        }

        public ObservableCollection<AlbumViewModel> Albums { get; }

        public ObservableCollection<PhotoViewModel> Photos { get; }

        public ICommand AddPhotosCommand { get; }

        public ICommand OpenGpsLink { get; }

        public ICommand DeletePhotoCommand { get; }

        public ICommand AddAlbumCommand { get; }

        public PhotoViewModel SelectedPhoto
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

        public async Task Load()
        {
            this.batchId = DateTime.Now.Date;

            Log.Debug($"Initial load. Batch ID is {batchId}");

            await Task.WhenAll(
                LoadAlbums(),
                LoadPhotos(20)
            );
        }

        public async Task LoadPhotos(int minLoadCount)
        {
            if (isLoading || batchId == DateTime.MinValue)
            {
                return;
            }

            isLoading = true;
            var numPhotos = 0;

            while (numPhotos < minLoadCount)
            {
                Log.Trace($"Loading photos. Batch ID is {batchId}");

                await foreach (var photo in this.photoMetadataStorage.GetPhotosByDate(this.batchId))
                {
                    var blob = await this.photoFileStorage.GetThumbnailAsync(photo.RowKey);
                    if (blob == null)
                    {
                        Log.Error($"Photo {photo.RowKey} was not found in storage.");
                        continue;
                    }

                    var thumbnail = GetImageFromBytes(blob.ToArray());

                    var viewModel = new PhotoViewModel(thumbnail, photo);

                    Log.Trace($"Adding {photo.RowKey}");

                    this.Photos.Add(viewModel);

                    numPhotos++;
                }

                batchId = batchId.AddDays(-1);
            }

            isLoading = false;
        }

        private async Task LoadAlbums()
        {
            this.Albums.Clear();

            await foreach (var album in this.albumStorage.GetPhotoAlbums())
            {
                var viewModel = new AlbumViewModel
                {
                    Title = album.AlbumName,
                    AlbumId = album.RowKey
                };

                string coverId =
                    string.IsNullOrEmpty(album.CoverPhotoId)
                    ? await this.settingsStorage.GetSetting("DefaultPhotoId")
                    : album.CoverPhotoId;

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
    }
}
