using AutoMapper;
using PhotoFox.Model;
using PhotoFox.Storage.Models;
using PhotoFox.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoFox.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoMetadataStorage photoMetadataStorage;

        private readonly IMapper mapper;

        public PhotoService(
            IPhotoMetadataStorage photoMetadataStorage,
            IMapper mapper)
        {
            this.photoMetadataStorage = photoMetadataStorage;
            this.mapper = mapper;
        }

        public async IAsyncEnumerable<Photo> GetPhotosByDateTaken(DateTime dateTaken)
        {
            await foreach (var photo in this.photoMetadataStorage.GetPhotosByDate(dateTaken))
            {
                yield return mapper.Map<Photo>(photo);
            }
        }

        public async Task<Photo> GetPhotoAsync(DateTime dateTaken, string photoId)
        {
            var metadata = await this.photoMetadataStorage.GetPhotoMetadata(dateTaken, photoId);
            return mapper.Map<Photo>(metadata);
        }

        public async Task SavePhotoAsync(Photo photo)
        {
            await this.photoMetadataStorage.SavePhotoAsync(mapper.Map<PhotoMetadata>(photo));
        }
    }
}
