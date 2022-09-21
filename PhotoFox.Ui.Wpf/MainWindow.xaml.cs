using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using NLog;
using PhotoFox.Wpf.Ui.Mvvm.Messages;
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
    public partial class MainWindow : Window, IRecipient<AddPhotosMessage>
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private MainWindowViewModel viewModel;

        public MainWindow(
            MainWindowViewModel viewModel,
            IMessenger messenger)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            this.DataContext = viewModel;

            messenger.Register<AddPhotosMessage>(this);
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
                await viewModel.LoadPhotos();
            }
        }

        public void Receive(AddPhotosMessage message)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.png)|*.jpg;*.png|All Files(*.*)|*.*",
                Title = "Open images",
                Multiselect = true
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                message.FileNames.AddRange(dialog.FileNames);
            }
        }
    }
}
