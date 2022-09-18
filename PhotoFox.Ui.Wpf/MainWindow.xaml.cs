﻿using NLog;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.Windows;
using System.Windows.Controls;
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

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            this.viewModel = viewModel;
            this.DataContext = viewModel;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("Main window loaded");

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
    }
}
