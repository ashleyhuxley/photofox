namespace PhotoFox.Model
{
    public class PhotoAlbum
    {
        public PhotoAlbum(
            string albumId, 
            string title, 
            string description, 
            string coverPhotoId,
            string folder)
        {
            this.AlbumId = albumId;
            this.Title = title;
            this.Description = description;
            this.CoverPhotoId = coverPhotoId;
            this.Folder = folder;
        }

        public string AlbumId { get; }

        public string Title { get; }

        public string Description { get; }

        public string CoverPhotoId { get; }

        public string Folder { get; }
    }
}
