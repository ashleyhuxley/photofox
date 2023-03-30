using NLog;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

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

        private async void PhotoList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var listView = (ListView)sender;
            var border = VisualTreeHelper.GetChild(listView, 0) as Decorator;

            if (border == null)
            {
                return;
            }

            var scrollViewer = border.Child as ScrollViewer;

            if (scrollViewer == null)
            {
                return;
            }

            if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                await viewModel.LoadMore();
            }
        }

        private void ItemDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            viewModel.OpenSelectedImage();
        }
    }
}
