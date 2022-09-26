using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace PhotoFox.Ui.Wpf
{
    public class MessageHandler :
        IMessageHandler,
        IRecipient<AddPhotosMessage>,
        IRecipient<OpenLinkMessage>,
        IRecipient<AddAlbumMessage>,
        IRecipient<UserConfirmMessage>
    {
        private readonly IMessenger messenger;

        private Window ownerWindow;

        public MessageHandler(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        public void Register(Window ownerWindow)
        {
            this.ownerWindow = ownerWindow;
            messenger.Register<AddPhotosMessage>(this);
            messenger.Register<OpenLinkMessage>(this);
            messenger.Register<AddAlbumMessage>(this);
            messenger.Register<UserConfirmMessage>(this);
        }

        public void Receive(AddPhotosMessage message)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.png)|*.jpg;*.png|All Files(*.*)|*.*",
                Title = "Open images",
                Multiselect = true
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                message.FileNames.AddRange(dialog.FileNames);
            }
        }

        public void Receive(OpenLinkMessage message)
        {
            OpenUrl(message.Link);
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public void Receive(AddAlbumMessage message)
        {
            var window = new AddAlbumWindow();
            window.Owner = this.ownerWindow;
            window.ShowDialog();
        }

        public void Receive(UserConfirmMessage message)
        {
            var result = MessageBox.Show(message.MessageText, message.Caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            message.IsConfirmed = result == MessageBoxResult.Yes;
        }
    }
}
