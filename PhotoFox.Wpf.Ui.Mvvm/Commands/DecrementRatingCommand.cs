using PhotoFox.Services;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class DecrementRatingCommand : ICommand
    {
        private readonly IPhotoService photoService;

        public DecrementRatingCommand(IPhotoService photoService)
        {
            this.photoService= photoService;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            var selectedPhotos = parameter as IEnumerable<PhotoViewModel>;
            if (selectedPhotos == null)
            {
                return;
            }

            foreach (var photoViewModel in selectedPhotos)
            {
                var newRating = await photoService.DecrementRatingAsync(photoViewModel.Item.DateTaken, photoViewModel.Item.PhotoId);
                photoViewModel.OverrideStarRating(newRating);
            }
        }
    }
}
