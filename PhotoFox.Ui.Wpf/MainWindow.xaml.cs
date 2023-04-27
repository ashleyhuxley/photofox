using Ninject;
using NLog;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Window
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private MainWindowViewModel viewModel;

        private IMessageHandler messageHandler;

        private IKernel kernel;

        public MainWindow(
            MainWindowViewModel viewModel,
            IMessageHandler messageHandler,
            IKernel kernel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            this.DataContext = viewModel;
            this.kernel = kernel;

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
            view.SortDescriptions.Add(new SortDescription("SortOrder", ListSortDirection.Descending));
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
            else if (e.Key == Key.Oem4)
            {
                viewModel.DecrementRatingCommand.Execute(viewModel.SelectedPhotos);
            }
            else if (e.Key == Key.Oem6)
            {
                viewModel.IncrementRatingCommand.Execute(viewModel.SelectedPhotos);
            }
        }

        private void VideoItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                viewModel.DeleteVideoCommand.Execute(viewModel.SelectedVideos);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                && e.Key == Key.P)
            {
                viewModel.SetPermissionsCommand.Execute(null);
            }
        }

        private void PhotoList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length == 1 && Path.GetExtension(files[0]).ToLowerInvariant() ==".json")
                {
                    var importViewModel = kernel.Get<ImportAlbumViewModel>();

                    var window = new ImportAlbumWindow
                    {
                        Owner = this,
                        DataContext = importViewModel
                    };

                    importViewModel.ImportFile = files[0];
                    window.Show();
                }
                else
                {
                    var uploadViewModel = kernel.Get<UploadFilesViewModel>();

                    var window = new UploadFilesWindow
                    {
                        Owner = this,
                        DataContext = uploadViewModel
                    };

                    uploadViewModel.AddFiles(files, viewModel.SelectedAlbum?.AlbumId ?? Guid.Empty.ToString());
                    window.Show();
                }
            }

            e.Handled = true;
        }
    }
}
