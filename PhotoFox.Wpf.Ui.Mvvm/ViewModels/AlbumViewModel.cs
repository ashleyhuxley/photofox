using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace PhotoFox.Ui.Wpf.Mvvm.ViewModels
{
    public class AlbumViewModel : ObservableObject
    {
        private bool isSelected;

        public string? Title { get; set; }

        public BitmapSource? Image { get; set; }

        public string? AlbumId { get; set; }

        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                if (value == this.isSelected)
                {
                    return;
                }

                this.isSelected = value;
                this.OnPropertyChanged(nameof(this.IsSelected));
            }
        }

        internal void SetImage(BitmapSource image)
        {
            this.Image = image;

            this.OnPropertyChanged(nameof(Image));
        }
    }
}
