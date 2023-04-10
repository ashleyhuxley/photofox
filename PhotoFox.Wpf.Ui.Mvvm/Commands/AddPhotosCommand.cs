using CommunityToolkit.Mvvm.Messaging;
using NLog;
using PhotoFox.Services;
using PhotoFox.Wpf.Ui.Mvvm.Commands.Parameters;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class AddPhotosCommand : ICommand
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly IMessenger messenger;

        private readonly IUploadService uploadService;

        public AddPhotosCommand(
            IMessenger messenger,
            IUploadService uploadService)
        {
            this.messenger = messenger;
            this.uploadService = uploadService;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            var parameters = parameter as AddPhotoCommandParameters;
            if (parameters == null)
            {
                throw new InvalidOperationException("AddPhotosCommand must be called with AddPhotosCommandParameters");
            }

            var i = 0;

            if (parameters.Files.Any())
            {
                foreach (var file in parameters.Files)
                {
                    i++;
                    await UploadImage(file, parameters.AlbumId);
                    this.messenger.Send(new SetStatusMessage($"Uploaded {file}"));
                }
            }
        }

        private async Task UploadImage(string fileName, string albumId)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            Log.Debug($"Uploading {fileName}");

            var info = new FileInfo(fileName);

            using (var stream = File.Open(fileName, FileMode.Open))
            {
                try
                {
                    await this.uploadService.UploadFromStreamAsync(stream, albumId, Path.GetFileName(fileName), Path.GetExtension(fileName).Substring(1), info.CreationTimeUtc);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Could not upload {fileName} - {ex.Message}");
                    return;
                }
            }
            
            File.Delete(fileName);
        }
    }
}
