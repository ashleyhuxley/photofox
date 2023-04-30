using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm
{
    public interface IHasThumbnail
    {
        BitmapSource? Image { get; set; }
    }
}
