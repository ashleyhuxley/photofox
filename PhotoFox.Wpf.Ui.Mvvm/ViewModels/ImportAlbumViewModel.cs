using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using PhotoFox.Model;
using PhotoFox.Services;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class ImportAlbumViewModel : ObservableObject
    {
        private readonly IPhotoAlbumService photoAlbumService;
        private readonly IUploadService uploadService;
        private readonly IMessenger messenger;

        private string albumName = string.Empty;
        private string description = string.Empty;

        public ImportAlbumViewModel(
            IPhotoAlbumService photoAlbumService,
            IUploadService uploadService,
            IMessenger messenger)
        {
            this.photoAlbumService = photoAlbumService;
            this.uploadService = uploadService;
            this.messenger = messenger;

            Photos = new ObservableCollection<PhotoImportViewModel>();
        }

        public ICommand ImportCommand => new AsyncRelayCommand(this.Import);

        public string? ImportFile { get; set; }

        public string AlbumName
        {
            get => this.albumName;
            set
            {
                this.albumName = value;
                OnPropertyChanged(nameof(AlbumName));
            }
        }

        public string Description
        {
            get => this.description;
            set
            {
                this.description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public ObservableCollection<PhotoImportViewModel> Photos { get; }

        public void Load()
        {
            if (ImportFile == null)
            {
                throw new InvalidOperationException("No import file specified.");
            }

            if (!File.Exists(ImportFile))
            {
                return;
            }

            var content = File.ReadAllText(ImportFile);
            var albumToImport = JsonConvert.DeserializeObject<AlbumImport>(content);

            if (albumToImport == null)
            {
                return;
            }

            this.AlbumName = albumToImport.Name ?? "New Album";
            this.Description = albumToImport.Description ?? string.Empty;
            if (albumToImport.Photos != null)
            {
                foreach (var photo in albumToImport.Photos)
                {
                    this.Photos.Add(new PhotoImportViewModel(photo));
                }
            }
        }

        public void OpenSelectedPhoto()
        {
            var photo = this.Photos.FirstOrDefault(p => p.IsSelected);
            if (photo == null)
            {
                return;
            }

            var message = new OpenPhotoMessage(GetFullPath(photo.Model), false);
            messenger.Send(message);
        }

        private async Task Import()
        {
            if (ImportFile == null)
            {
                return;
            }

            var album = new PhotoAlbum(Guid.NewGuid().ToString(), this.AlbumName, this.Description, string.Empty, "Imported");

            await this.photoAlbumService.AddAlbumAsync(album);

            foreach (var photo in this.Photos)
            {
                await ProcessPhotoAsync(photo, album.AlbumId);
            }

            File.Delete(ImportFile);
        }

        private string GetFullPath(PhotoImport photoImport)
        {
            if (ImportFile == null || photoImport?.Uri == null)
            {
                return string.Empty;
            }

            var fileInfo = new FileInfo(ImportFile);
            var root = fileInfo.Directory?.Parent?.Parent;
            if (root == null || photoImport.Uri == null)
            {
                return string.Empty;
            }

            return Path.Combine(root.FullName, photoImport.Uri.Replace('/', '\\'));
        }

        private async Task ProcessPhotoAsync(PhotoImportViewModel photo, string albumId)
        {
            if (ImportFile == null)
            {
                photo.Status = UploadStatus.Failed;
                return;
            }

            var filePath = GetFullPath(photo.Model);
            if (string.IsNullOrEmpty(filePath))
            {
                photo.Status = UploadStatus.Failed;
                return;
            }

            using (var stream = File.OpenRead(filePath))
            {
                photo.Status = UploadStatus.InProgress;

                try
                {
                    await uploadService.UploadFromStreamAsync(
                        stream,
                        albumId,
                        photo.Model.Title ?? string.Empty,
                        Path.GetExtension(filePath).Substring(1),
                        photo.Model.Date ?? DateTime.UtcNow);

                    photo.Status = UploadStatus.Success;
                }
                catch (Exception ex)
                {
                    photo.Status = UploadStatus.Failed;
                }
            }

            File.Delete(filePath);
        }
    }
}