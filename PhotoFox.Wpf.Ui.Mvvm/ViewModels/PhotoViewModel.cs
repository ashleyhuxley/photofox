﻿using PhotoFox.Model;
using System;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class PhotoViewModel
    {
        public Photo Photo { get; private set; }

        public PhotoViewModel(BitmapImage image, Photo photo)
        {
            Image = image;
            Photo = photo;
        }

        public BitmapImage Image { get; }

        public string GroupName => this.Photo.DateTaken.ToLongDateString();

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
    }
}
