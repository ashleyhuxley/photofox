using PhotoFox.Wpf.Ui.Mvvm.ViewModels;

namespace PhotoFox.Wpf.Ui.Mvvm.Messages
{
    public class UnloadVideoMessage
    {
        public UnloadVideoMessage(VideoViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public VideoViewModel ViewModel { get; }
    }
}
