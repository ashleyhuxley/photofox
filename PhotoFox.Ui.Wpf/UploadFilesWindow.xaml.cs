using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.Windows;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for UploadFilesWindow.xaml
    /// </summary>
    public partial class UploadFilesWindow : Window
    {
        public UploadFilesWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as UploadFilesViewModel;
            if (viewModel == null)
            {
                return;
            }

            await viewModel.StartAsync();
        }
    }
}
