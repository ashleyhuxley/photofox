using PhotoFox.Ui.Wpf.Mvvm.ViewModels;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class UnloadAlbumMessage
    {
        public UnloadAlbumMessage(AlbumViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public AlbumViewModel ViewModel { get; }
    }
}
