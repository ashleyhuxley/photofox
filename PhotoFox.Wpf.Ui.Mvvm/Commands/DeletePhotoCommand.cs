using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class DeletePhotoCommand : ICommand
    {
        private readonly IPhotoFileStorage photoFileStorage;

        private readonly IPhotoMetadataStorage photoMetadataStorage;

        private readonly IPhotoHashStorage photoHashStorage;

        public DeletePhotoCommand(
            IPhotoFileStorage photoFileStorage, 
            IPhotoMetadataStorage photoMetadataStorage,
            IPhotoHashStorage photoHashStorage)
        {
            this.photoFileStorage = photoFileStorage;
            this.photoMetadataStorage = photoMetadataStorage;
            this.photoHashStorage = photoHashStorage;
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

            await Task.WhenAll(
                this.photoFileStorage.DeleteThumbnailAsync(selectedPhoto.RowKey),
                this.photoFileStorage.DeletePhotoAsync(selectedPhoto.RowKey),
                this.photoMetadataStorage.DeletePhotoAsync(selectedPhoto.PartitionKey, selectedPhoto.RowKey),
                this.photoHashStorage.DeleteHashAsync(selectedPhoto.FileHash));
        }
    }
}
