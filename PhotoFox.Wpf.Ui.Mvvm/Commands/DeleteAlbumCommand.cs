﻿using CommunityToolkit.Mvvm.Messaging;
using PhotoFox.Services;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
using System;
using System.Windows.Input;

namespace PhotoFox.Wpf.Ui.Mvvm.Commands
{
    public class DeleteAlbumCommand : ICommand
    {
        private readonly IMessenger messenger;

        private readonly IPhotoAlbumService photoAlbumService;

        public DeleteAlbumCommand(
            IPhotoAlbumService IPhotoAlbumService,
            IMessenger messenger)
        {
            this.photoAlbumService = IPhotoAlbumService;
            this.messenger = messenger;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            var viewModel = parameter as AlbumViewModel;
            return viewModel != null;
        }

        public void Execute(object? parameter)
        {
            var viewModel = parameter as AlbumViewModel;
            if (viewModel?.AlbumId == null)
            {
                return;
            }

            this.photoAlbumService.DeleteAlbumAsync(viewModel.AlbumId);
            this.messenger.Send(new UnloadAlbumMessage(viewModel));
        }
    }
}
