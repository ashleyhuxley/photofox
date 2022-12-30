using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using PhotoFox.Storage.Blob;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using Ninject;

namespace PhotoFox.Ui.Wpf
{
    public class MessageHandler :
        IMessageHandler,
        IRecipient<AddPhotosMessage>,
        IRecipient<OpenLinkMessage>,
        IRecipient<AddAlbumMessage>,
        IRecipient<UserConfirmMessage>,
        IRecipient<OpenPhotoMessage>,
        IRecipient<SelectAlbumMessage>,
        IRecipient<ShowPermissionsWindowMessage>
    {
        private readonly IMessenger messenger;

        private readonly IPhotoFileStorage photoStorage;

        private readonly IKernel kernel;

        private Window? ownerWindow;

        public MessageHandler(
            IMessenger messenger,
            IPhotoFileStorage photoStorage,
            IKernel kernel)
        {
            this.messenger = messenger;
            this.photoStorage = photoStorage;
            this.kernel = kernel;
        }

        public void Register(Window ownerWindow)
        {
            this.ownerWindow = ownerWindow;
            messenger.Register<AddPhotosMessage>(this);
            messenger.Register<OpenLinkMessage>(this);
            messenger.Register<AddAlbumMessage>(this);
            messenger.Register<UserConfirmMessage>(this);
            messenger.Register<OpenPhotoMessage>(this);
            messenger.Register<SelectAlbumMessage>(this);
            messenger.Register<ShowPermissionsWindowMessage>(this);
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
            EnsureOwnerWindowIsSet();

            var window = new AddAlbumWindow
            {
                Owner = this.ownerWindow,
                DataContext = message.ViewModel
            };

            var result = window.ShowDialog();

            message.DialogResult = result;
        }

        public void Receive(UserConfirmMessage message)
        {
            var result = MessageBox.Show(message.MessageText, message.Caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
            message.IsConfirmed = result == MessageBoxResult.Yes;
        }

        public async void Receive(OpenPhotoMessage message)
        {
            var photo = await this.photoStorage.GetPhotoAsync(message.PhotoId);
            var path = Path.GetTempFileName() + ".jpg";

            File.WriteAllBytes(path, photo.ToArray());

            var proc = new Process();
            proc.StartInfo.FileName = "C:\\Program Files\\IrfanView\\i_view64.exe";
            proc.StartInfo.Arguments = path;
            proc.Start();
        }

        private void EnsureOwnerWindowIsSet()
        {
            if (this.ownerWindow is null)
            {
                throw new InvalidOperationException("Cannot handle window related messages without owner window being set. Please call Register first.");
            }
        }

        public void Receive(SelectAlbumMessage message)
        {
            var viewModel = this.kernel.Get<SelectAlbumViewModel>();

            viewModel.Load();

            var window = new SelectAlbumWindow
            {
                Owner = this.ownerWindow,
                DataContext = viewModel
            };

            var result = window.ShowDialog();

            message.Reply(new SelectAlbumMessageResponse
            {
                SelectedAlbumId = viewModel.SelectedAlbum.AlbumId,
                Result = result.HasValue && result.Value,
                NewAlbumName = viewModel.NewAlbumName
            });
        }

        public void Receive(ShowPermissionsWindowMessage message)
        {
            var viewModel = this.kernel.Get<AlbumPermissionsViewModel>();

            viewModel.Load();

            var window = new AlbumPermissionsWindow
            {
                Owner = this.ownerWindow,
                DataContext = viewModel
            };

            window.ShowDialog();
        }
    }
}
