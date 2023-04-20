using System.Windows;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for EditAlbumWindow.xaml
    /// </summary>
    public partial class EditAlbumWindow : Window
    {
        public EditAlbumWindow()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
