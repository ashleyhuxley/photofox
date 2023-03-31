using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Services;
using PhotoFox.Model;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class AddAlbumCommand : ICommand
    {
        private readonly IMessenger messenger;

        private readonly IPhotoAlbumService photoAlbumService;

        public AddAlbumCommand(
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
            return parameter is PhotoViewModel;
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
                    AlbumId = Guid.NewGuid().ToString(),
                    Title = message.ViewModel.AlbumName,
                    CoverPhotoId = selectedPhoto.Photo.PhotoId
                };

                this.photoAlbumService.AddAlbumAsync(album);

                this.photoAlbumService.AddPhotoToAlbumAsync(album.AlbumId, selectedPhoto.Photo.PhotoId, selectedPhoto.Photo.DateTaken);
            }

            this.messenger.Send(new RefreshAlbumsMessage());
        }
    }
}
