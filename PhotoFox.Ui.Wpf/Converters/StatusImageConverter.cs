using PhotoFox.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PhotoFox.Ui.Wpf.Converters
{
    [ValueConversion(typeof(UploadStatus), typeof(string))]
    public class StatusImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((UploadStatus)value)
            {
                case UploadStatus.Success:
                    return "/Images/accept.png";
                case UploadStatus.InProgress:
                    return "/Images/hourglass.png";
                case UploadStatus.Failed:
                    return "/Images/cancel.png";
                case UploadStatus.Ready:
                    return "/Images/picture.png";
            }

            throw new ArgumentOutOfRangeException(nameof(value), "Value is not a valid status");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
