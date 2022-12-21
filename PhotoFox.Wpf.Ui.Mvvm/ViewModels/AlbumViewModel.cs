using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Windows.Media.Imaging;

namespace PhotoFox.Ui.Wpf.Mvvm.ViewModels
{
    public class AlbumViewModel : ObservableObject
    {
        public string Title { get; set; }

        public BitmapImage Image { get; set; }

        public string AlbumId { get; set; }

        internal void SetImage(BitmapImage image, Rotation rotation)
        {
            this.Image = image;
            this.Image.Rotation = rotation;

            this.OnPropertyChanged(nameof(Image));
        }
    }
}
