using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class SetPermissionsCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly IMessenger messenger;

        public SetPermissionsCommand(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var message = new ShowPermissionsWindowMessage();
            this.messenger.Send(message);
        }
    }
}
