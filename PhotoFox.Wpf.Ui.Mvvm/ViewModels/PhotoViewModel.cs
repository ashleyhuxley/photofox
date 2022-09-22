using PhotoFox.Model;
using System;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class PhotoViewModel
    {
        private PhotoMetadata Metadata { get; set; }

        public PhotoViewModel(BitmapImage image, PhotoMetadata metadata)
        {
            Image = image;
            Metadata = metadata;
        }

        public BitmapImage Image { get; }
        public string GroupName => this.DateTaken.ToLongDateString();

        public string Title
        {
            get => this.Metadata.Title;
            set => this.Metadata.Title = value;
        }

        public DateTime DateTaken => this.Metadata.UtcDate;
        public string Description
        {
            get => this.Metadata.Description;
            set => this.Metadata.Description = value;
        }

        public string Exposure => this.Metadata.Exposure;
        public string Apeture => this.Metadata.Aperture;
        public string Device => this.Metadata.Device;

        public double Latitude => this.Metadata.GeolocationLattitude.Value;
        public double Longitude => this.Metadata.GeolocationLongitude.Value;

        public string GpsCoords
        {
            get
            {
                if (Metadata.GeolocationLongitude.HasValue && Metadata.GeolocationLattitude.HasValue)
                {
                    return $"{this.Metadata.GeolocationLattitude:0.#####}, {this.Metadata.GeolocationLongitude:0.#####}";
                }

                return string.Empty;
            }
        }
        
    }
}
