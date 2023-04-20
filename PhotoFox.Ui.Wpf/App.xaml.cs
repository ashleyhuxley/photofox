using CommunityToolkit.Mvvm.Messaging;
using Ninject;
using PhotoFox.Core.Hashing;
using PhotoFox.Core.Imaging;
using PhotoFox.Services;
using PhotoFox.Storage;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Queue;
using PhotoFox.Storage.Table;
using PhotoFox.Wpf.Ui.Mvvm.Commands;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.Runtime.Versioning;
using System.Windows;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class App : Application
    {
        private IKernel? container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        
        private void ConfigureContainer()
        {
            this.container = new StandardKernel();

            this.container.Bind<MainWindowViewModel>().ToSelf();
            this.container.Bind<UploadFilesViewModel>().ToSelf();
            this.container.Bind<ImportAlbumViewModel>().ToSelf();

            this.container.Bind<IStorageConfig>().To<PhotoFoxConfig>().InSingletonScope();
            this.container.Bind<IViewerConfig>().To<PhotoFoxConfig>().InSingletonScope();
            this.container.Bind<IMessenger>().To<WeakReferenceMessenger>().InSingletonScope();
            this.container.Bind<IThumbnailProvider>().To<ThumbnailProvider>().InSingletonScope();
            this.container.Bind<IStreamHash>().To<StreamHashMD5>().InSingletonScope();
            this.container.Bind<IMessageHandler>().To<MessageHandler>().InSingletonScope();
            this.container.Bind<PhotoFox.Wpf.Ui.Mvvm.IContext>().To<WpfContext>();

            // Services
            this.container.Bind<IUploadService>().To<UploadService>().InSingletonScope();
            this.container.Bind<IPhotoAlbumService>().To<PhotoAlbumService>().InSingletonScope();
            this.container.Bind<IPhotoService>().To<PhotoService>().InSingletonScope();
            this.container.Bind<IVideoService>().To<VideoService>().InSingletonScope();

            // Storage
            this.container.Bind<IPhotoAlbumDataStorage>().To<PhotoAlbumDataStorage>().InSingletonScope();
            this.container.Bind<IPhotoFileStorage>().To<PhotoFileStorage>().InSingletonScope();
            this.container.Bind<IPhotoMetadataStorage>().To<PhotoMetadataStorage>().InSingletonScope();
            this.container.Bind<IPhotoInAlbumStorage>().To<PhotoInAlbumStorage>().InSingletonScope();
            this.container.Bind<IVideoInAlbumStorage>().To<VideoInAlbumStorage>().InSingletonScope();
            this.container.Bind<IPhotoHashStorage>().To<PhotoHashStorage>().InSingletonScope();
            this.container.Bind<IAlbumPermissionStorage>().To<AlbumPermissionStorage>().InSingletonScope();
            this.container.Bind<IUserStorage>().To<UserStorage>().InSingletonScope();
            this.container.Bind<IUploadQueue>().To<UploadQueue>().InSingletonScope();
            this.container.Bind<IVideoStorage>().To<VideoStorage>().InSingletonScope();
            this.container.Bind<IUploadStorage>().To<UploadStorage>().InSingletonScope();

            // Commands
            this.container.Bind<AddPhotosCommand>().ToSelf();
            this.container.Bind<OpenGpsLocationCommand>().ToSelf();
            this.container.Bind<DeletePhotoCommand>().ToSelf();
            this.container.Bind<AddAlbumCommand>().ToSelf();
            this.container.Bind<DeleteAlbumCommand>().ToSelf();
            this.container.Bind<SaveChangesCommand>().ToSelf();
            this.container.Bind<EditSelectedAlbumCommand>().ToSelf();
        }

        private void ComposeObjects()
        {
            Current.MainWindow = this.container.Get<MainWindow>();
        }
    }
}
