using PhotoFox.Wpf.Ui.Mvvm.ViewModels;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class AddAlbumMessage
    {
        public AddAlbumMessage()
        {
            ViewModel = new AddAlbumViewModel();
        }

        public bool? DialogResult { get; set; }

        public AddAlbumViewModel ViewModel { get; set; }
    }
}
