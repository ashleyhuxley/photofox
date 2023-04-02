using NLog;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.ComponentModel;
using System.Windows;
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

            SetupPhotoSort();
            SetupAlbumSort();

            await this.viewModel.Load();
        }

        private void SetupPhotoSort()
        {
            var view = (CollectionView)CollectionViewSource.GetDefaultView(PhotoList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("GroupName");
            view.GroupDescriptions.Add(groupDescription);

            view.SortDescriptions.Add(new SortDescription("GroupSort", ListSortDirection.Descending));
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
    }
}
