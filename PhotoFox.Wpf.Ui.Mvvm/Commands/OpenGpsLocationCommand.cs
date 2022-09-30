using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class OpenGpsLocationCommand : ICommand
    {
        private readonly IMessenger messenger;

        public OpenGpsLocationCommand(IMessenger messenger)
        {
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

        public void Execute(object? parameter)
        {
            var selectedPhoto = parameter as PhotoViewModel;
            if (selectedPhoto == null)
            {
                return;
            }

            this.messenger.Send(new OpenLinkMessage($"https://maps.google.com/?q={selectedPhoto.Photo.GeolocationLattitude},{selectedPhoto.Photo.GeolocationLongitude}"));
        }
    }
}
