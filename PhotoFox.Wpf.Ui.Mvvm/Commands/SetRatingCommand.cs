using PhotoFox.Services;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class SetRatingCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly IPhotoService photoService;

        public SetRatingCommand(IPhotoService photoService)
        {
            this.photoService = photoService;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            var objs = parameter as Tuple<int, IEnumerable<PhotoViewModel>>;
            if (objs == null)
            {
                return;
            }

            var selectedPhotos = objs.Item2;
            foreach (var photoViewModel in selectedPhotos)
            {
                await photoService.SetRatingAsync(photoViewModel.Item.DateTaken, photoViewModel.Item.PhotoId, objs.Item1);
            }
        }
    }
}
