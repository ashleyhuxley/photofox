﻿using PhotoFox.Wpf.Ui.Mvvm.ViewModels;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class UnloadAlbumMessage
    {
        public UnloadAlbumMessage(AlbumViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public AlbumViewModel ViewModel { get; }
    }
}
