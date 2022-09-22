﻿using ExifLibrary;
using PhotoFox.Core.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PhotoFox.Core.Exif
{
    public class ExifReader
    {
        private ImageFile imageFile;

        private ExifReader(ImageFile imageFile)
        {
            this.imageFile = imageFile;
        }

        public static async Task<ExifReader> FromStreamAsync(Stream stream)
        {
            return new ExifReader(await ImageFile.FromStreamAsync(stream));
        }

        public string GetIso()
        {
            var iso = this.imageFile.Properties.Get<ExifUShort>(ExifTag.ISOSpeedRatings);
            if (iso != null)
            {
                return iso.Value.ToString();
            }

            return string.Empty;
        }

        public DateTime? GetDateTakenUtc()
        {
            var dateTaken = this.imageFile.Properties.Get<ExifDateTime>(ExifTag.DateTime);
            if (dateTaken != null)
            {
                return dateTaken.Value.ToUniversalTime();
            }

            return null;
        }

        public string GetApeture()
        {
            var apeture = this.imageFile.Properties.Get<ExifURational>(ExifTag.ApertureValue);
            if (apeture != null)
            {
                return Math.Round(Math.Pow(2, apeture.GetValue() / 2), 1).ToString();
            }

            return string.Empty;
        }

        public string GetFocalLength()
        {
            var focalLength = this.imageFile.Properties.Get<ExifURational>(ExifTag.FocalLength);
            if (focalLength != null)
            {
                return Math.Round((double)focalLength.Value.Numerator / focalLength.Value.Denominator, 2).ToString();
            }

            return string.Empty;
        }

        public string GetModel()
        {
            var model = this.imageFile.Properties.Get<ExifAscii>(ExifTag.Model);
            if (model != null)
            {
                return model.Value;
            }

            return string.Empty;
        }

        public int? GetOrientation()
        {
            var orientation = this.imageFile.Properties.Get<ExifUShort>(ExifTag.Orientation);
            if (orientation != null)
            {
                return orientation.Value;
            }

            return null;
        }

        public string GetExposure()
        {
            var exposure = this.imageFile.Properties.Get<ExifURational>(ExifTag.ExposureTime);
            if (exposure != null)
            {
                return exposure.Value.Numerator.ToString() + " / " + exposure.Value.Denominator.ToString();
            }

            return string.Empty;
        }

        public double? GetGpsLatitude()
        {
            var gpsLat = this.imageFile.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLatitude);
            var gpsLatRef = this.imageFile.Properties.Get<ExifEnumProperty<GPSLatitudeRef>>(ExifTag.GPSLatitudeRef);

            if (gpsLatRef != null && gpsLatRef != null)
            {
                var latDecimal = ConvertDegreeAngleToDouble(gpsLat) * (gpsLatRef.Value == GPSLatitudeRef.North ? 1 : -1);
                if (latDecimal != 0)
                {
                    return latDecimal;
                }
            }

            return null;
        }

        public double? GetGpsLongitude()
        {
            var gpsLon = this.imageFile.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLongitude);
            var gpsLonRef = this.imageFile.Properties.Get<ExifEnumProperty<GPSLongitudeRef>>(ExifTag.GPSLongitudeRef);

            if (gpsLon != null)
            {
                var lonDecimal = ConvertDegreeAngleToDouble(gpsLon) * (gpsLonRef.Value == GPSLongitudeRef.East ? 1 : -1);
                if (lonDecimal != 0)
                {
                    return lonDecimal;
                }
            }

            return null;
        }

        private static double ConvertDegreeAngleToDouble(GPSLatitudeLongitude gps)
        {
            var degrees = (double)gps.Degrees.Numerator / gps.Degrees.Denominator;
            var minutes = (double)gps.Minutes.Numerator / gps.Minutes.Denominator;
            var seconds = (double)gps.Seconds.Numerator / gps.Seconds.Denominator;

            //Decimal degrees = 
            //   whole number of degrees, 
            //   plus minutes divided by 60, 
            //   plus seconds divided by 3600

            return degrees + minutes / 60 + seconds / 3600;
        }
    }
}