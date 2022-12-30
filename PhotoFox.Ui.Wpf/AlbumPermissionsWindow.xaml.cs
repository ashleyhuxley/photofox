using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for AlbumPermissionsWindow.xaml
    /// </summary>
    public partial class AlbumPermissionsWindow : Window
    {
        public AlbumPermissionsWindow()
        {
            InitializeComponent();
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
