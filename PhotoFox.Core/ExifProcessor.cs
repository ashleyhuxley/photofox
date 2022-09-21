using ExifLibrary;
using PhotoFox.Core.Extensions;
using PhotoFox.Model;
using System;

namespace PhotoFox.Core
{
    public static class ExifProcessor
    {
        public static void SetExifData(string fileName, PhotoMetadata metadata)
        {
            var image = ImageFile.FromFile(fileName);

            var iso = image.Properties.Get<ExifUShort>(ExifTag.ISOSpeedRatings);
            var dateTaken = image.Properties.Get<ExifDateTime>(ExifTag.DateTime);
            var apeture = image.Properties.Get<ExifURational>(ExifTag.ApertureValue);
            var focalLength = image.Properties.Get<ExifURational>(ExifTag.FocalLength);
            var model = image.Properties.Get<ExifAscii>(ExifTag.Model);
            var orientation = image.Properties.Get<ExifUShort>(ExifTag.Orientation);
            var exposure = image.Properties.Get<ExifURational>(ExifTag.ExposureTime);
            var gpsLat = image.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLatitude);
            var gpsLon = image.Properties.Get<GPSLatitudeLongitude>(ExifTag.GPSLongitude);
            var gpsLatRef = image.Properties.Get<ExifEnumProperty<GPSLatitudeRef>>(ExifTag.GPSLatitudeRef);
            var gpsLonRef = image.Properties.Get<ExifEnumProperty<GPSLongitudeRef>>(ExifTag.GPSLongitudeRef);

            if (iso != null) metadata.Iso = iso.Value.ToString();
            if (dateTaken != null) metadata.DateTaken = dateTaken.Value;
            if (focalLength != null) metadata.FocalLength = Math.Round(((double)focalLength.Value.Numerator / focalLength.Value.Denominator), 2).ToString();
            if (apeture != null) metadata.Aperture = Math.Round(Math.Pow(2, apeture.GetValue() / 2), 1).ToString();
            if (model != null) metadata.Device = model.Value;
            if (orientation != null) metadata.Orientation = orientation.Value;
            if (exposure != null) metadata.Exposure = exposure.Value.Numerator.ToString() + " / " + exposure.Value.Denominator.ToString();
            if (gpsLat != null)
            {
                var latDecimal = ConvertDegreeAngleToDouble(gpsLat) * (gpsLatRef.Value == GPSLatitudeRef.North ? 1 : -1);
                if (latDecimal != 0)
                {
                    metadata.GeolocationLattitude = latDecimal;
                }
            }
            if (gpsLon != null)
            {
                var lonDecimal = ConvertDegreeAngleToDouble(gpsLon) * (gpsLonRef.Value == GPSLongitudeRef.East ? 1 : -1);
                if (lonDecimal != 0)
                {
                    metadata.GeolocationLongitude = lonDecimal;
                }
            }
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

            return degrees + (minutes / 60) + (seconds / 3600);
        }
    }
}
