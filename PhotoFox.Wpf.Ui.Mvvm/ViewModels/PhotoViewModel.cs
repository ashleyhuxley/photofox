using System;
using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class PhotoViewModel
    {
        public string Title { get; set; }

        public BitmapImage Image { get; set; }

        public DateTime DateTime { get; set; }

        public string GroupName => this.DateTime.ToLongDateString();
    }
}
