using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using PhotoFox.Ui.Wpf.Mvvm.ViewModels;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IPhotoDataStorage photoStorage;

        private readonly IPhotoFileStorage photoFileStorage;

        public MainWindowViewModel(
            IPhotoDataStorage photoStorage,
            IPhotoFileStorage photoFileStorage)
        {
            this.photoStorage = photoStorage;
            this.photoFileStorage = photoFileStorage;

            this.Albums = new ObservableCollection<AlbumViewModel>();
        }

        public ObservableCollection<AlbumViewModel> Albums { get; }

        public async Task Load()
        {
            this.Albums.Clear();

            await foreach (var album in this.photoStorage.GetPhotoAlbums())
            {
                var viewModel = new AlbumViewModel();
                viewModel.Title = album.AlbumName;

                if (!string.IsNullOrEmpty(album.CoverPhotoId))
                {
                    var blob = await this.photoFileStorage.GetFileAsync(album.CoverPhotoId);
                    viewModel.Image = GetImageFromBytes(blob.ToArray());
                }

                this.Albums.Add(viewModel);
            }
        }

        private BitmapImage GetImageFromBytes(byte[] bytes)
        {
            System.IO.MemoryStream Stream = new System.IO.MemoryStream();
            Stream.Write(bytes, 0, bytes.Length);
            Stream.Position = 0;
            Image img = Image.FromStream(Stream);
            BitmapImage bitImage = new BitmapImage();
            bitImage.BeginInit();
            System.IO.MemoryStream MS = new System.IO.MemoryStream();
            img.Save(MS, ImageFormat.Jpeg);
            MS.Seek(0, System.IO.SeekOrigin.Begin);
            bitImage.StreamSource = MS;
            bitImage.EndInit();
            return bitImage;
        }
    }
}
