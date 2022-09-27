using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class AddAlbumCommand : ICommand
    {
        private readonly IMessenger messenger;

        private readonly IPhotoAlbumDataStorage photoAlbumDataStorage;

        private readonly IPhotoInAlbumStorage photoInAlbumStorage;

        public AddAlbumCommand(
            IMessenger messenger,
            IPhotoAlbumDataStorage photoAlbumDataStorage,
            IPhotoInAlbumStorage photoInAlbumStorage)
        {
            this.messenger = messenger;
            this.photoAlbumDataStorage = photoAlbumDataStorage;
            this.photoInAlbumStorage = photoInAlbumStorage;
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

        public void Execute(object? parameter)
        {
            var selectedPhoto = parameter as PhotoViewModel;
            if (selectedPhoto == null)
            {
                return;
            }

            var message = this.messenger.Send(new AddAlbumMessage());
            if (message.DialogResult.HasValue && message.DialogResult.Value)
            {
                var album = new PhotoAlbum
                {
                    PartitionKey = "photoalbum",
                    RowKey = Guid.NewGuid().ToString(),
                    AlbumName = message.ViewModel.AlbumName,
                    CoverPhotoId = selectedPhoto.Metadata.RowKey
                };

                this.photoAlbumDataStorage.AddPhotoAlbum(album);

                this.photoInAlbumStorage.AddPhotoInAlbumAsync(album.RowKey, selectedPhoto.Metadata.RowKey);
            }

            this.messenger.Send(new RefreshAlbumsMessage());
        }
    }
}
