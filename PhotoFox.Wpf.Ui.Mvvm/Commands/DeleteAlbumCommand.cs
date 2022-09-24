using PhotoFox.Storage.Table;
using PhotoFox.Ui.Wpf.Mvvm.ViewModels;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class DeleteAlbumCommand : ICommand
    {
        private readonly IPhotoAlbumDataStorage photoAlbumDataStorage;

        public DeleteAlbumCommand(IPhotoAlbumDataStorage photoAlbumDataStorage)
        {
            this.photoAlbumDataStorage = photoAlbumDataStorage;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            var viewModel = parameter as AlbumViewModel;
            return viewModel == null;
        }

        public void Execute(object? parameter)
        {
            var viewModel = parameter as AlbumViewModel;
            if (viewModel == null)
            {
                return;
            }

            this.photoAlbumDataStorage.DeleteAlbumAsync(viewModel.AlbumId);
        }
    }
}
