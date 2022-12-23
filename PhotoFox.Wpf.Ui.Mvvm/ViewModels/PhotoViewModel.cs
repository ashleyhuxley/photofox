using CommunityToolkit.Mvvm.ComponentModel;
using PhotoFox.Model;
using PhotoFox.Core.Extensions;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class PhotoViewModel : ObservableObject
    {
        private bool isSelected;

        private TransformedBitmap? image;

        public Photo Photo { get; private set; }

        public PhotoViewModel(Photo photo)
        {
            Photo = photo;
        }

        public TransformedBitmap? Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(this.Image));
            }
        }

        public string GroupName => this.Photo.DateTaken.ToLongDateString();

        public string GroupSort => this.Photo.DateTaken.ToString("yyyy-MM-dd");

        public string GpsCoords
        {
            get
            {
                if (Photo.GeolocationLongitude.HasValue && Photo.GeolocationLattitude.HasValue)
                {
                    return $"{this.Photo.GeolocationLattitude:0.#####}, {this.Photo.GeolocationLongitude:0.#####}";
                }

                return string.Empty;
            }
        }

        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                this.isSelected = value;
                this.OnPropertyChanged(nameof(IsSelected));
            }
        }

        public string Dimensions => $"{this.Photo.DimensionWidth} x {this.Photo.DimensionHeight}";

        public string FileSize => this.Photo.FileSize.HasValue ? this.Photo.FileSize.Value.ToFileSize() : "Unknown";
    }
}
