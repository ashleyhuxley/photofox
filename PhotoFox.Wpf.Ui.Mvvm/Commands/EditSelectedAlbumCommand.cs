using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.Windows.Input;
using PhotoFox.Services;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class EditSelectedAlbumCommand : ICommand
    {
        private readonly IMessenger messenger;

        private readonly IPhotoAlbumService photoAlbumService;

        public EditSelectedAlbumCommand(
            IMessenger messenger,
            IPhotoAlbumService photoAlbumService)
        { 
            this.messenger = messenger;
            this.photoAlbumService = photoAlbumService;
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

        public async void Execute(object? parameter)
        {
            var viewModel = parameter as AlbumViewModel;
            if (viewModel?.AlbumId == null)
            {
                return;
            }

            var message = new EditAlbumMessage(viewModel);
            this.messenger.Send(message);

            if (message.DialogResult.GetValueOrDefault(false))
            {
                await this.photoAlbumService.EditAlbumAsync(viewModel.AlbumId, viewModel.Title, viewModel.Description, viewModel.Folder);
            }
        }
    }
}
