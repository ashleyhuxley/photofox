namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class OpenPhotoMessage
    {
        public OpenPhotoMessage(string photoId)
        {
            this.PhotoId = photoId;
            IsId = true;
        }

        public OpenPhotoMessage(string id, bool isId)
        {
            this.PhotoId= id;
            this.IsId = isId;
        }

        public string PhotoId { get; set; }

        public bool IsId { get; }
    }
}
