using PhotoFox.Model;

namespace PhotoFox.Wpf.Ui.Mvvm.ViewModels
{
    public class PhotoViewModel : ItemViewModelBase<Photo>
    {
        public PhotoViewModel(Photo photo)
            : base(photo)
        {
        }

        public string Dimensions => $"{this.Item.DimensionWidth} x {this.Item.DimensionHeight}";
    }
}
