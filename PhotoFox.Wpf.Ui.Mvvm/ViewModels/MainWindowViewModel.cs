﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NLog;
using PhotoFox.Core;
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

            AddPhotosCommand = new AsyncRelayCommand(AddPhotosExecute);
        }

        public ObservableCollection<AlbumViewModel> Albums { get; }

        public ObservableCollection<PhotoViewModel> Photos { get; }

        public ICommand AddPhotosCommand { get; }

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
                Log.Trace($"Loading photos. Batch ID is {batchId}");

                await foreach (var photo in this.photoMetadataStorage.GetPhotosByDate(this.batchId))
                {
                    var viewModel = new PhotoViewModel
                    {
                        Title = photo.UtcDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                        DateTime = photo.UtcDate.ToLocalTime(),
                    };

                    var blob = await this.photoFileStorage.GetThumbnailAsync(photo.RowKey);
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

        private async Task AddPhotosExecute()
        {
            var message = new AddPhotosMessage();
            this.messenger.Send(message);

            if (message.FileNames.Any())
            {
                foreach (var file in message.FileNames)
                {
                    await UploadImage(file);
                }
            }
        }

        private async Task UploadImage(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            Log.Debug($"Uploading {fileName}");

            var metatdata = new PhotoMetadata();

            ExifProcessor.SetExifData(fileName, metatdata);

            metatdata.PartitionKey = metatdata.UtcDate.ToPartitionKey();
            metatdata.RowKey = Guid.NewGuid().ToString();

            Log.Trace($"PK: {metatdata.PartitionKey}, RK: {metatdata.RowKey}");

            var thumbnail = ThumbnailProcessor.GetThumbnail(Image.FromFile(fileName), 250);

            using (var ms = new MemoryStream())
            {
                thumbnail.Save(ms, ImageFormat.Jpeg);

                ms.Seek(0, SeekOrigin.Begin);
                var data = await BinaryData.FromStreamAsync(ms);
                await this.photoFileStorage.PutThumbnailAsync(metatdata.RowKey, data);
            }

            await this.photoFileStorage.PutPhotoAsync(metatdata.RowKey, BinaryData.FromBytes(File.ReadAllBytes(fileName)));

            await this.photoMetadataStorage.AddPhotoAsync(metatdata);
        }
    }
}
