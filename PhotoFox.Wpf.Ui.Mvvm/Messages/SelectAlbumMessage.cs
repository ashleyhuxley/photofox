using CommunityToolkit.Mvvm.Messaging.Messages;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class SelectAlbumMessage : RequestMessage<SelectAlbumMessageResponse>
    {

    }

    public class SelectAlbumMessageResponse
    {
        public string? SelectedAlbumId { get; set; }

        public bool Result { get; set; }

        public string? NewAlbumName { get; set; }
    }
}
