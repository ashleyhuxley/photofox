using PhotoFox.Wpf.Ui.Mvvm.ViewModels;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class UnloadPhotoMessage
    {
        public UnloadPhotoMessage(PhotoViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public PhotoViewModel ViewModel { get; }
    }
}
