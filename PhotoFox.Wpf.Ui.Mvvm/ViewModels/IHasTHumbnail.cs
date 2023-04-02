using System.Windows.Media.Imaging;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public interface IHasTHumbnail
    {
        BitmapSource? Image { get; set; }
    }
}
