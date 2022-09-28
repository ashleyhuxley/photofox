using PhotoFox.Model;
using PhotoFox.Storage.Table;
using System.Collections.Generic;

namespace PhotoFox.Services
{
    public class PhotoAlbumService : IPhotoAlbumService
    {
        private readonly IPhotoAlbumDataStorage photoAlbumDataStorage;

        public PhotoAlbumService(IPhotoAlbumDataStorage photoAlbumDataStorage)
        {
            this.photoAlbumDataStorage = photoAlbumDataStorage;
        }

        public async IAsyncEnumerable<PhotoAlbum> GetAllAlbums()
        {
            await foreach (var album in this.photoAlbumDataStorage.GetPhotoAlbums())
            {
                yield return new PhotoAlbum
                {
                    AlbumId = album.RowKey,
                    CoverPhotoId = album.CoverPhotoId,
                    Description = album.AlbumDescription,
                    Title = album.AlbumName
                };
            }
        }
    }
}
