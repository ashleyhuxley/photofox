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
        private IKernel contianer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ComposeObjects();
            Current.MainWindow.Show();
        }

        private IMapper AutoMapper(IContext context)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PhotoMetadata, Photo>()
                    .ForMember(dest => dest.PhotoId, opt => opt.MapFrom(src => src.RowKey))
                    .ForMember(dest => dest.DateTaken, opt => opt.MapFrom(src => src.UtcDate));
                cfg.CreateMap<Storage.Models.PhotoAlbum, Model.PhotoAlbum>();
                cfg.CreateMap<Photo, PhotoMetadata>()
                    .ForMember(dest => dest.RowKey, opt => opt.MapFrom(src => src.PhotoId))
                    .ForMember(dest => dest.UtcDate, opt => opt.MapFrom(src => src.DateTaken))
                    .ForMember(dest => dest.PartitionKey, opt => opt.MapFrom(src => src.DateTaken.ToPartitionKey()));
                cfg.CreateMap<PhotoAlbum, Storage.Models.PhotoAlbum>()
                    .ForMember(dest => dest.CoverPhotoId, opt => opt.MapFrom(src => src.CoverPhotoId))
                    .ForMember(dest => dest.AlbumName, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.AlbumDescription, opt => opt.MapFrom(src => src.Description))
                    .ForMember(dest => dest.PartitionKey, opt => opt.MapFrom(src => src.AlbumId))
                    .ForMember(dest => dest.RowKey, opt => opt.MapFrom(src => string.Empty));
            });

            var mapper = new Mapper(config);

            return mapper;
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
