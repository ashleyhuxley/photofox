using PhotoFox.Model;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class LoadPhotoMessage
    {
        public LoadPhotoMessage(Photo photo)
        {
            this.Photo = photo;
        }

        public Photo Photo { get; }
    }
}
