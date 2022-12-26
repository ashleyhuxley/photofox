using AutoMapper;
using PhotoFox.Core.Extensions;
using PhotoFox.Model;
using PhotoFox.Storage.Models;
using PhotoAlbum = PhotoFox.Model.PhotoAlbum;

namespace PhotoFox.Mappings
{
    public static class MapFactory
    {
        public static IMapper GetMap()
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
    }
}