namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class OpenVideoMessage
    {
        public OpenVideoMessage(string videoId, string fileExt)
        {
            this.VideoId = videoId;
            this.FileExt = fileExt;
        }

        public string VideoId { get; set; }

        public string FileExt { get; set; }
    }
}
