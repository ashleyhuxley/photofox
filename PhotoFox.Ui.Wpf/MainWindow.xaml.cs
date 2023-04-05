using NLog;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private MainWindowViewModel viewModel;

        private IMessageHandler messageHandler;

        public MainWindow(
            MainWindowViewModel viewModel,
            IMessageHandler messageHandler)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            this.DataContext = viewModel;

            this.messageHandler = messageHandler;
            this.messageHandler.Register(this);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("Main window loaded");

            SetupSort(VideoList, "GroupName", "GroupSort");
            SetupSort(PhotoList, "GroupName", "GroupSort");
            SetupAlbumSort();

            await this.viewModel.Load();
        }

        private void SetupSort(ListView listView, string groupName, string sortName)
        {
            var view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription(groupName);
            view.GroupDescriptions.Add(groupDescription);

            view.SortDescriptions.Add(new SortDescription(sortName, ListSortDirection.Descending));
        }

        private void SetupAlbumSort()
        {
            var view = (CollectionView)CollectionViewSource.GetDefaultView(AlbumList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        }

        private void PhotoDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            viewModel.OpenSelectedImage();
        }

        private void VideoItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.OpenSelectedVideo();
        }

        private void PhotoItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                viewModel.DeletePhotoCommand.Execute(viewModel.SelectedPhotos);
            }
        }

        private void VideoItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                viewModel.DeleteVideoCommand.Execute(viewModel.SelectedVideos);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                && e.Key == Key.P)
            {
                viewModel.SetPermissionsCommand.Execute(null);
            }
        }
    }
}
