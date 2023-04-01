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

            this.contianer.Bind<IStorageConfig>().To<PhotoFoxConfig>();
            this.contianer.Bind<IMessenger>().To<WeakReferenceMessenger>().InSingletonScope();
            this.contianer.Bind<IThumbnailProvider>().To<ThumbnailProvider>();
            this.contianer.Bind<IStreamHash>().To<StreamHashMD5>();
            this.contianer.Bind<IMessageHandler>().To<MessageHandler>();
            this.contianer.Bind<PhotoFox.Wpf.Ui.Mvvm.IContext>().To<WpfContext>();

            // Services
            this.contianer.Bind<IUploadService>().To<UploadService>();
            this.contianer.Bind<IPhotoAlbumService>().To<PhotoAlbumService>();
            this.contianer.Bind<IPhotoService>().To<PhotoService>();

            // Storage
            this.contianer.Bind<IPhotoAlbumDataStorage>().To<PhotoAlbumDataStorage>();
            this.contianer.Bind<IPhotoFileStorage>().To<PhotoFileStorage>();
            this.contianer.Bind<IPhotoMetadataStorage>().To<PhotoMetadataStorage>();
            this.contianer.Bind<IPhotoInAlbumStorage>().To<PhotoInAlbumStorage>();
            this.contianer.Bind<IPhotoHashStorage>().To<PhotoHashStorage>();
            this.contianer.Bind<IAlbumPermissionStorage>().To<AlbumPermissionStorage>();
            this.contianer.Bind<IUserStorage>().To<UserStorage>();
            this.contianer.Bind<IUploadQueue>().To<UploadQueue>();

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
