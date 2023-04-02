using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Services;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class DeletePhotoCommand : ICommand
    {
        private readonly IPhotoService photoService;

        private readonly IMessenger messenger;

        public DeletePhotoCommand(
            IPhotoService photoService,
            IMessenger messenger)
        {
            this.photoService = photoService;
            this.messenger = messenger;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return parameter is IEnumerable<PhotoViewModel>;
        }

        public async void Execute(object? parameter)
        {
            var selectedPhotos = parameter as IEnumerable<PhotoViewModel>;
            if (selectedPhotos == null)
            {
                return;
            }

            var msg = this.messenger.Send(new UserConfirmMessage("Are you sure you want to delete the selected photos?", "Warning"));
            if (!msg.IsConfirmed)
            {
                return;
            }

            var photosToRemove = new List<PhotoViewModel>();

            foreach (var selectedPhoto in selectedPhotos)
            {
                await this.photoService.DeletePhotoAsync(selectedPhoto.Item);

                photosToRemove.Add(selectedPhoto);
            }

            foreach (var selectedPhoto in photosToRemove)
            {
                this.messenger.Send(new UnloadPhotoMessage(selectedPhoto));
            }
        }
    }
}
