using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Media.Imaging;

namespace PhotoFox.Ui.Wpf.Mvvm.ViewModels
{
    public class AlbumViewModel : ObservableObject
    {
        public string Title { get; set; }

        public TransformedBitmap Image { get; set; }

        public string AlbumId { get; set; }

        internal void SetImage(TransformedBitmap image)
        {
            this.Image = image;

            this.OnPropertyChanged(nameof(Image));
        }
    }
}
