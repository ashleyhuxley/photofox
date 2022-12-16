namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class OpenPhotoMessage
    {
        public OpenPhotoMessage(string photoId)
        {
            this.PhotoId = photoId;
        }

        public string PhotoId { get; set; }
    }
}
