using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Storage.Table;
using PhotoFox.Ui.Wpf.Mvvm.ViewModels;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class DeleteAlbumCommand : ICommand
    {
        private readonly IPhotoAlbumDataStorage photoAlbumDataStorage;

        private readonly IMessenger messenger;

        public DeleteAlbumCommand(
            IPhotoAlbumDataStorage photoAlbumDataStorage,
            IMessenger messenger)
        {
            this.photoAlbumDataStorage = photoAlbumDataStorage;
            this.messenger = messenger;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            var viewModel = parameter as AlbumViewModel;
            return viewModel != null;
        }

        public void Execute(object? parameter)
        {
            var viewModel = parameter as AlbumViewModel;
            if (viewModel == null)
            {
                return;
            }

            this.photoAlbumDataStorage.DeleteAlbumAsync(viewModel.AlbumId);
            this.messenger.Send(new RefreshAlbumsMessage());
        }
    }
}
