using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Services;
using PhotoFox.Storage.Table;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class SaveChangesCommand : ICommand
    {
        private readonly IPhotoService photoService;

        private readonly IMessenger messenger;

        public SaveChangesCommand(
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

            await this.photoService.SavePhotoAsync(selectedPhoto.Item);

            var newMetadata = await this.photoService.GetPhotoAsync(selectedPhoto.Item.DateTaken, selectedPhoto.Item.PhotoId);

            this.messenger.Send(new UpdateStatusMessage("Image updated."));
            this.messenger.Send(new LoadPhotoMessage(newMetadata));
        }
    }
}
