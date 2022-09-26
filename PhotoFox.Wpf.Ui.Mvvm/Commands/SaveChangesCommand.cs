using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Storage.Table;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class SaveChangesCommand : ICommand
    {
        private readonly IPhotoMetadataStorage photoMetadataStorage;

        private readonly IMessenger messenger;

        public SaveChangesCommand(
            IPhotoMetadataStorage photoMetadataStorage,
            IMessenger messenger)
        {
            this.photoMetadataStorage = photoMetadataStorage;
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

            await this.photoMetadataStorage.SavePhotoAsync(selectedPhoto.Metadata);

            var newMetadata = await this.photoMetadataStorage.GetPhotoMetadata(selectedPhoto.DateTaken, selectedPhoto.RowKey);

            this.messenger.Send(new UpdateStatusMessage("Image updated."));
            this.messenger.Send(new LoadPhotoMessage(newMetadata));
        }
    }
}
