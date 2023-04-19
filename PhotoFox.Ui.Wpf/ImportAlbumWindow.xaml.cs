using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for ImportAlbumWindow.xaml
    /// </summary>
    public partial class ImportAlbumWindow : Window
    {
        public ImportAlbumWindow()
        {
            InitializeComponent();
        }

        public ImportAlbumViewModel? ViewModel => this.DataContext as ImportAlbumViewModel;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.Load();
        }

        public void PhotoDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.OpenSelectedPhoto();
        }
    }
}
