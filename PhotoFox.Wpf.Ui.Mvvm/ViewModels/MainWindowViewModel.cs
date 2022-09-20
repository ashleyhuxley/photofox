using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExifLibrary;
using NLog;
using PhotoFox.Core.Extensions;
using PhotoFox.Model;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using PhotoFox.Ui.Wpf.Mvvm.ViewModels;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class MainWindowViewModel
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IPhotoAlbumDataStorage albumStorage;

        private readonly IPhotoFileStorage photoFileStorage;

        private readonly ISettingsStorage settingsStorage;

        private readonly IPhotoMetadataStorage photoMetadataStorage;

        private readonly IMessenger messenger;

        private DateTime batchId = DateTime.MinValue;

        private bool isLoading = false;

        public MainWindowViewModel(
            IPhotoAlbumDataStorage photoStorage,
            IPhotoFileStorage photoFileStorage,
            ISettingsStorage settingsStorage,
            IPhotoMetadataStorage photoMetadataStorage,
            IMessenger messenger)
        {
            this.albumStorage = photoStorage;
            this.photoFileStorage = photoFileStorage;
            this.settingsStorage = settingsStorage;
            this.photoMetadataStorage = photoMetadataStorage;
            this.messenger = messenger;

            this.Albums = new ObservableCollection<AlbumViewModel>();
            this.Photos = new ObservableCollection<PhotoViewModel>();

            AddPhotosCommand = new RelayCommand(AddPhotosExecute);
        }

        public ObservableCollection<AlbumViewModel> Albums { get; }

        public ObservableCollection<PhotoViewModel> Photos { get; }

        public ICommand AddPhotosCommand;

        public async Task Load()
        {
            this.batchId = DateTime.Now.Date;

            Log.Debug($"Initial load. Batch ID is {batchId}");

            await Task.WhenAll(
                LoadAlbums(),
                LoadPhotos()
            );
        }

        public async Task LoadPhotos()
        {
            if (isLoading || batchId == DateTime.MinValue)
            {
                return;
            }

            isLoading = true;
            var numPhotos = 0;

            while (numPhotos == 0)
            {
                Log.Debug($"Loading photos. Batch ID is {batchId}");

                await foreach (var photo in this.photoMetadataStorage.GetPhotosByDate(this.batchId))
                {
                    var viewModel = new PhotoViewModel
                    {
                        Title = photo.DateTaken.ToString("dd/MM/yyyy HH:mm"),
                        DateTime = photo.DateTaken
                    };

                    var blob = await this.photoFileStorage.GetFileAsync(photo.RowKey);
                    if (blob != null)
                    {
                        viewModel.Image = GetImageFromBytes(blob.ToArray());
                    }

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

        private void AddPhotosExecute()
        {
            var message = new AddPhotosMessage();
            this.messenger.Send(message);

            if (message.FileNames.Any())
            {
                foreach (var file in message.FileNames)
                {
                    UploadImage(file);
                }
            }
        }

        private void UploadImage(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            var image = ImageFile.FromFile(fileName);

            var iso = image.Properties.Get<ExifUShort>(ExifTag.ISOSpeedRatings);
            var dateTaken = image.Properties.Get<ExifDateTime>(ExifTag.DateTime);
            var apeture = image.Properties.Get<ExifURational>(ExifTag.ApertureValue);
            var focalLength = image.Properties.Get<ExifURational>(ExifTag.FocalLength);
            var model = image.Properties.Get<ExifAscii>(ExifTag.Model);
            var orientation = image.Properties.Get<ExifUShort>(ExifTag.Orientation);
            var exposure = image.Properties.Get<ExifURational>(ExifTag.ExposureTime);
            var shutterSpeed = image.Properties.Get<ExifURational>(ExifTag.ShutterSpeedValue);
            var flash = image.Properties.Get<ExifUShort>(ExifTag.Flash);
            var gpsLat = image.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLatitude);
            var gpsLon = image.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLongitude);
            var gpsLatRef = image.Properties.Get<ExifEnumProperty<GPSLatitudeRef>>(ExifTag.GPSLatitudeRef);
            var gpsLonRef = image.Properties.Get<ExifEnumProperty<GPSLongitudeRef>>(ExifTag.GPSLongitudeRef);

            var metadata = new PhotoMetadata();

            if (iso != null) metadata.Iso = iso.Value.ToString();
            if (dateTaken != null) metadata.DateTaken = dateTaken.Value;
            if (focalLength != null) metadata.FocalLength = Math.Round(((double)focalLength.Value.Numerator / focalLength.Value.Denominator), 2).ToString();
            if (apeture != null) metadata.Aperture = Math.Round(Math.Pow(2, apeture.GetValue() / 2), 1).ToString();
            if (model != null) metadata.Device = model.Value;
            if (orientation != null) metadata.Orientation = orientation.Value;
            if (exposure != null) metadata.Exposure = exposure.Value.Numerator.ToString() + " / " + exposure.Value.Denominator.ToString();
            if (gpsLat != null) metadata.GeolocationLattitude = ConvertDegreeAngleToDouble(gpsLat) * (gpsLatRef.Value == GPSLatitudeRef.North ? 1 : -1);
            if (gpsLon != null) metadata.GeolocationLongitude = ConvertDegreeAngleToDouble(gpsLon) * (gpsLonRef.Value == GPSLongitudeRef.East ? 1 : -1);
        }

        public double ConvertDegreeAngleToDouble(GPSLatitudeLongitude gps)
        {
            var degrees = (double)gps.Degrees.Numerator / gps.Degrees.Denominator;
            var minutes = (double)gps.Minutes.Numerator / gps.Minutes.Denominator;
            var seconds = (double)gps.Seconds.Numerator / gps.Seconds.Denominator;

            //Decimal degrees = 
            //   whole number of degrees, 
            //   plus minutes divided by 60, 
            //   plus seconds divided by 3600

            return degrees + (minutes / 60) + (seconds / 3600);
        }


    }
}
