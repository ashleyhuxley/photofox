using PhotoFox.Model;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class LoadPhotoMessage
    {
        public LoadPhotoMessage(PhotoMetadata photoMetadata)
        {
            this.PhotoMetadata = photoMetadata;
        }

        public PhotoMetadata PhotoMetadata { get; }
    }
}
