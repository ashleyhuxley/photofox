using AutoMapper;
using CommunityToolkit.Mvvm.Messaging;
using Ninject;
using Ninject.Activation;
using PhotoFox.Core.Extensions;
using PhotoFox.Core.Hashing;
using PhotoFox.Core.Imaging;
using PhotoFox.Mappings;
using PhotoFox.Model;
using PhotoFox.Services;
using PhotoFox.Storage;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Queue;
using PhotoFox.Storage.Table;
using PhotoFox.Wpf.Ui.Mvvm.Commands;
using PhotoFox.Wpf.Ui.Mvvm.ViewModels;
using System.Windows;
using PhotoAlbum = PhotoFox.Model.PhotoAlbum;

namespace PhotoFox.Ui.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel? contianer;

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

            this.contianer.Bind<IMapper>().ToMethod(i => MapFactory.GetMap()).InSingletonScope();

            this.contianer.Bind<MainWindowViewModel>().ToSelf();
            this.contianer.Bind<UploadFilesViewModel>().ToSelf();

            this.contianer.Bind<IStorageConfig>().To<PhotoFoxConfig>().InSingletonScope();
            this.contianer.Bind<IViewerConfig>().To<PhotoFoxConfig>().InSingletonScope();
            this.contianer.Bind<IMessenger>().To<WeakReferenceMessenger>().InSingletonScope();
            this.contianer.Bind<IThumbnailProvider>().To<ThumbnailProvider>().InSingletonScope();
            this.contianer.Bind<IStreamHash>().To<StreamHashMD5>().InSingletonScope();
            this.contianer.Bind<IMessageHandler>().To<MessageHandler>().InSingletonScope();
            this.contianer.Bind<PhotoFox.Wpf.Ui.Mvvm.IContext>().To<WpfContext>();

            // Services
            this.contianer.Bind<IUploadService>().To<UploadService>().InSingletonScope();
            this.contianer.Bind<IPhotoAlbumService>().To<PhotoAlbumService>().InSingletonScope();
            this.contianer.Bind<IPhotoService>().To<PhotoService>().InSingletonScope();
            this.contianer.Bind<IVideoService>().To<VideoService>().InSingletonScope();

            // Storage
            this.contianer.Bind<IPhotoAlbumDataStorage>().To<PhotoAlbumDataStorage>().InSingletonScope();
            this.contianer.Bind<IPhotoFileStorage>().To<PhotoFileStorage>().InSingletonScope();
            this.contianer.Bind<IPhotoMetadataStorage>().To<PhotoMetadataStorage>().InSingletonScope();
            this.contianer.Bind<IPhotoInAlbumStorage>().To<PhotoInAlbumStorage>().InSingletonScope();
            this.contianer.Bind<IVideoInAlbumStorage>().To<VideoInAlbumStorage>().InSingletonScope();
            this.contianer.Bind<IPhotoHashStorage>().To<PhotoHashStorage>().InSingletonScope();
            this.contianer.Bind<IAlbumPermissionStorage>().To<AlbumPermissionStorage>().InSingletonScope();
            this.contianer.Bind<IUserStorage>().To<UserStorage>().InSingletonScope();
            this.contianer.Bind<IUploadQueue>().To<UploadQueue>().InSingletonScope();
            this.contianer.Bind<IVideoStorage>().To<VideoStorage>().InSingletonScope();

            // Commands
            this.contianer.Bind<AddPhotosCommand>().ToSelf();
            this.contianer.Bind<OpenGpsLocationCommand>().ToSelf();
            this.contianer.Bind<DeletePhotoCommand>().ToSelf();
            this.contianer.Bind<AddAlbumCommand>().ToSelf();
            this.contianer.Bind<DeleteAlbumCommand>().ToSelf();
            this.contianer.Bind<SaveChangesCommand>().ToSelf();
        }

        private void ComposeObjects()
        {
            Current.MainWindow = this.contianer.Get<MainWindow>();
        }
    }
}
