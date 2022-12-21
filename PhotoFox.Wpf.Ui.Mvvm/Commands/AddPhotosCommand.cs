﻿using CommunityToolkit.Mvvm.Messaging;
using NLog;
using PhotoFox.Services;
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
            var message = new AddPhotosMessage();
            this.messenger.Send(message);

            var i = 0;

            if (message.FileNames.Any())
            {
                foreach (var file in message.FileNames)
                {
                    i++;
                    await UploadImage(file);
                    this.messenger.Send(new SetStatusMessage($"Uploaded file {i} of {message.FileNames.Count}"));
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

            var info = new FileInfo(fileName);

            using (var stream = File.Open(fileName, FileMode.Open))
            {
                try
                {
                    var photo = await this.uploadService.UploadFromStreamAsync(stream, info.CreationTimeUtc, Path.GetFileName(fileName));
                    if (photo != null)
                    {
                        this.messenger.Send(new LoadPhotoMessage(photo));
                    }
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
