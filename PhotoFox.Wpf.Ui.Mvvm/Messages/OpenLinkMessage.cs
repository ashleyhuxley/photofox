namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class OpenLinkMessage
    {
        public OpenLinkMessage(string link)
        {
            Link = link;
        }

        public string Link { get; set; }
    }
}
