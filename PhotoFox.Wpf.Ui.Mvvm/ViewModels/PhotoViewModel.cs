using PhotoFox.Model;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class PhotoViewModel : ItemViewModelBase<Photo>
    {
        public PhotoViewModel(Photo photo)
            : base(photo)
        {
        }

        public string Dimensions => $"{this.Item.ImageProperties.Dimensions.Width} x {this.Item.ImageProperties.Dimensions.Height}";
    }
}
