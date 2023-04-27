using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotoFox.Model;
using PhotoFox.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class UploadFilesViewModel : ObservableObject
    {
        private readonly IUploadService _uploadService;

        private string albumId = Guid.Empty.ToString();

        public UploadFilesViewModel(IUploadService uploadService)
        {
            _uploadService = uploadService;

            this.Uploads = new ObservableCollection<UploadStatusViewModel>();
        }

        public ObservableCollection<UploadStatusViewModel> Uploads { get; }

        public ICommand CleanUpCommand => new RelayCommand(CleanUpExecute);

        public void AddFiles(string[] files, string albumId)
        {
            this.albumId = albumId;

            foreach (var file in files)
            {
                Uploads.Add(new UploadStatusViewModel(file));
            }
        }

        public async Task StartAsync()
        {
            var tasks = Uploads
                .Select(o => ProcessFile(o, albumId))
                .ToArray();

            await Task.WhenAll(tasks);
        }

        private async Task ProcessFile(UploadStatusViewModel viewModel, string albumId)
        {
            if (!File.Exists(viewModel.Filename))
            {
                viewModel.Status = UploadStatus.Failed;
                viewModel.Error = "File not found";
                return;
            }

            var ext = Path.GetExtension(viewModel.Filename);
            var type = GetTypeFromExtension(ext);
            if (type == UploadType.Other)
            {
                viewModel.Status = UploadStatus.Failed;
                viewModel.Error = "Unsupported file type";
                return;
            }

            viewModel.Status = UploadStatus.InProgress;

            var fileInfo = new FileInfo(viewModel.Filename);
            var title = Path.GetFileName(viewModel.Filename);
            if (title.Length > 25)
            {
                title = title.Substring(0, 25);
            }

            if (type == UploadType.Photo)
            {
                using (var stream = File.OpenRead(viewModel.Filename))
                {
                    await _uploadService.UploadFromStreamAsync(stream, albumId, title, Path.GetExtension(viewModel.Filename).Substring(1), fileInfo.CreationTimeUtc);
                    viewModel.Status = UploadStatus.Success;
                }
            }
            else if (type == UploadType.Video)
            {
                using (var stream = File.OpenRead(viewModel.Filename))
                {
                    await _uploadService.UploadVideoFromStreamAsync(stream, albumId, title, Path.GetExtension(viewModel.Filename).Substring(1), fileInfo.CreationTimeUtc);
                    viewModel.Status = UploadStatus.Success;
                }
            }
        }

        private UploadType GetTypeFromExtension(string extension)
        {
            switch (extension.ToUpperInvariant())
            {
                case ".JPG":
                case ".JPEG":
                case ".BMP":
                case ".PNG":
                    return UploadType.Photo;
                case ".MP4":
                case ".MKV":
                case ".AVI":
                case ".M4V":
                    return UploadType.Video;
                default:
                    return UploadType.Other;
            }
        }

        private void CleanUpExecute()
        {
            var toRemove = new List<UploadStatusViewModel>();

            foreach (var upload in Uploads)
            {
                File.Delete(upload.Filename);
                toRemove.Add(upload);
            }

            toRemove.ForEach(t => Uploads.Remove(t));
        }
    }
}
