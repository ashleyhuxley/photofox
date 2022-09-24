using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class AddAlbumCommand : ICommand
    {
        private readonly IMessenger messenger;

        public AddAlbumCommand(IMessenger messenger)
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
            return true;
        }

        public void Execute(object? parameter)
        {
            this.messenger.Send(new AddAlbumMessage());
        }
    }
}
