using NLog;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

            var view = (CollectionView)CollectionViewSource.GetDefaultView(PhotoList.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("GroupName");
            view.GroupDescriptions.Add(groupDescription);

            await this.viewModel.Load();
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

        private async void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //var sv = FindVisualChild<ScrollViewer>(PhotoList);
            //while (!sv.CanContentScroll)
            //{
            //    await this.viewModel.LoadPhotos();
            //}
        }

        public static T FindVisualChild<T>(DependencyObject obj)
            where T : DependencyObject
        {
            // Iterate through all immediate children
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is T)
                    return (T)child;

                else
                {
                    var childOfChild = FindVisualChild<T>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
    }
}
