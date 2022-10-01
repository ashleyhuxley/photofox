using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Services;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
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
            return parameter != null && parameter is PhotoViewModel;
        }

        public async void Execute(object? parameter)
        {
            var selectedPhoto = parameter as PhotoViewModel;
            if (selectedPhoto == null)
            {
                return;
            }

            var msg = this.messenger.Send(new UserConfirmMessage("Are you sure you want to delete the selected photo?", "Warning"));
            if (!msg.IsConfirmed)
            {
                return;
            }

            await this.photoService.DeletePhotoAsync(selectedPhoto.Photo);

            this.messenger.Send(new UnloadPhotoMessage(selectedPhoto));
        }
    }
}
