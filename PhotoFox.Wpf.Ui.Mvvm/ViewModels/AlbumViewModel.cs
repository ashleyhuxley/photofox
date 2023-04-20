using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class AlbumViewModel : ObservableObject
    {
        private bool isSelected;

        public AlbumViewModel()
        {
            this.Title = string.Empty;
            this.AlbumId = string.Empty;
            this.Folder = string.Empty;
            this.Description = string.Empty;
            this.SortOrder= string.Empty;
        }

        public string Title { get; set; }

        public BitmapSource? Image { get; set; }

        public string AlbumId { get; set; }

        public string Folder { get; set; }

        public string Description { get; set; }

        public string SortOrder { get; set; }

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
