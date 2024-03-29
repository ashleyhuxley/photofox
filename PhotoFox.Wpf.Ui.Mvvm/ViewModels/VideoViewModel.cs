﻿using PhotoFox.Model;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class VideoViewModel : ItemViewModelBase<Video>, IHasThumbnail
    {
        public VideoViewModel(Video item) : base(item)
        {
        }
    }
}
