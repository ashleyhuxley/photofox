using CommunityToolkit.Mvvm.ComponentModel;
using PhotoFox.Core.Extensions;
using PhotoFox.Model;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class ItemViewModelBase<T> : ObservableObject
        where T : IDisplayableItem
    {
        private bool isSelected;

        private BitmapSource? image;

        private T item;

        public ItemViewModelBase(T item)
        {
            this.item = item;
        }

        public T Item
        {
            get => this.item;
            set
            {
                this.item = value;
                this.OnPropertyChanged(nameof(this.Title));
            }
        }

        public BitmapSource? Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(this.Image));
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

        public string GroupName => this.Item.DateTaken.ToLongDateString();

        public string GroupSort => this.Item.DateTaken.ToString("yyyy-MM-dd");

        public string GpsCoords
        {
            get
            {
                if (this.Item.GeolocationLongitude.HasValue && this.Item.GeolocationLatitude.HasValue)
                {
                    return $"{this.Item.GeolocationLatitude:0.#####}, {this.Item.GeolocationLongitude:0.#####}";
                }

                return string.Empty;
            }
        }

        public string Title => this.Item.Title;

        public string FileSize => this.Item.FileSize.HasValue ? this.Item.FileSize.Value.ToFileSize() : "Unknown";
    }
}
