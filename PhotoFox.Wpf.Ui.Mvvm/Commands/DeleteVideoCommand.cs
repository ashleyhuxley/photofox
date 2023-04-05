using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Services;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class DeleteVideoCommand : ICommand
    {
        private readonly IVideoService videoService;

        private readonly IMessenger messenger;

        public DeleteVideoCommand(
            IVideoService videoService,
            IMessenger messenger)
        {
            this.videoService = videoService;
            this.messenger = messenger;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return parameter is IEnumerable<VideoViewModel>;
        }

        public async void Execute(object? parameter)
        {
            var selectedVideos = parameter as IEnumerable<VideoViewModel>;
            if (selectedVideos == null)
            {
                return;
            }

            var msg = this.messenger.Send(new UserConfirmMessage("Are you sure you want to delete the selected photos?", "Warning"));
            if (!msg.IsConfirmed)
            {
                return;
            }

            var photosToRemove = new List<VideoViewModel>();

            foreach (var selectedVideo in selectedVideos)
            {
                await this.videoService.DeleteVideoAsync(selectedVideo.Item);

                photosToRemove.Add(selectedVideo);
            }

            foreach (var selectedVideo in photosToRemove)
            {
                this.messenger.Send(new UnloadVideoMessage(selectedVideo));
            }
        }
    }
}
