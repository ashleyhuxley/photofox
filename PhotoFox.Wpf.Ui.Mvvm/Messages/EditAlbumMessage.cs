using PhotoFox.Wpf.Ui.Mvvm.ViewModels;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class EditAlbumMessage
    {
        public EditAlbumMessage(AlbumViewModel viewModel)
        { 
            this.ViewModel = viewModel;
        }

        public AlbumViewModel ViewModel { get; }

        public bool? DialogResult { get; set; }
    }
}
