using NLog;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using PhotoFox.Ui.Wpf.Mvvm.ViewModels;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class MainWindowViewModel
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoAlbumDataStorage albumStorage;

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly IPhotoInBatchStorage photoInBatchStorage;

        private readonly ISettingsStorage settingsStorage;

        private int batchId;

        private bool isLoading = false;

        public MainWindowViewModel(
            IPhotoAlbumDataStorage photoStorage,
            IPhotoFileStorage photoFileStorage,
            ISettingsStorage settingsStorage,
            IPhotoInBatchStorage photoInBatchStorage)
        {
            this.albumStorage = photoStorage;
            this.photoFileStorage = photoFileStorage;
            this.settingsStorage = settingsStorage;
            this.photoInBatchStorage = photoInBatchStorage;

            this.Albums = new ObservableCollection<AlbumViewModel>();
            this.Photos = new ObservableCollection<PhotoViewModel>();
        }

        public ObservableCollection<AlbumViewModel> Albums { get; }

        public ObservableCollection<PhotoViewModel> Photos { get; }

        public async Task Load()
        {
            this.batchId = int.Parse(await settingsStorage.GetSetting("LatestBatchId"));

            Log.Debug($"Initial load. Batch ID is {batchId}");

            await Task.WhenAll(
                LoadAlbums(),
                LoadPhotos()
            );
        }

        public async Task LoadPhotos()
        {
            if (isLoading || batchId == 0)
            {
                return;
            }

            isLoading = true;

            Log.Debug($"Loading photos. Batch ID is {batchId}");

            await foreach (var photo in this.photoInBatchStorage.GetPhotosInBatch(this.batchId))
            {
                var viewModel = new PhotoViewModel();
                viewModel.Title = photo.RowKey;

                var blob = await this.photoFileStorage.GetFileAsync(photo.RowKey);
                if (blob != null)
                {
                    viewModel.Image = GetImageFromBytes(blob.ToArray());
                }

                Log.Trace($"Adding {photo.RowKey}");

                this.Photos.Add(viewModel);
            }

            batchId--;
            isLoading = false;
        }

        private async Task LoadAlbums()
        {
            this.Albums.Clear();

            await foreach (var album in this.albumStorage.GetPhotoAlbums())
            {
                var viewModel = new AlbumViewModel();
                viewModel.Title = album.AlbumName;

                string coverId =
                    string.IsNullOrEmpty(album.CoverPhotoId)
                    ? await this.settingsStorage.GetSetting("DefaultPhotoId")
                    : album.CoverPhotoId;

                var blob = await this.photoFileStorage.GetFileAsync(coverId);
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
