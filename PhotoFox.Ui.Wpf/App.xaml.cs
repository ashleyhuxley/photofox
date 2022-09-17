using Ninject;
using PhotoFox.Storage;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.Windows;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel contianer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        private void ConfigureContainer()
        {
            this.contianer = new StandardKernel();

            this.contianer.Bind<MainWindowViewModel>().ToSelf();

            this.contianer.Bind<IPhotoDataStorage>().To<PhotoDataStorage>();
            this.contianer.Bind<IPhotoFileStorage>().To<PhotoFileStorage>();
            this.contianer.Bind<IStorageConfig>().To<PhotoFoxConfig>();
        }

        private void ComposeObjects()
        {
            Current.MainWindow = this.contianer.Get<MainWindow>();
        }
    }
}
