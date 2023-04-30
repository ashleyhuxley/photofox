using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class AlbumViewModel : ObservableObject, IHasThumbnail
    {
        private bool isSelected;

        private BitmapSource? image;

        public AlbumViewModel()
        {
            this.Title = string.Empty;
            this.AlbumId = string.Empty;
            this.Folder = string.Empty;
            this.Description = string.Empty;
            this.SortOrder= string.Empty;
            this.CoverPhotoId = string.Empty;
        }

        public string Title { get; set; }

        public BitmapSource? Image
        {
            get => this.image;
            set
            {
                this.image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public string AlbumId { get; set; }

        public string CoverPhotoId { get; set; }

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
    }
}
