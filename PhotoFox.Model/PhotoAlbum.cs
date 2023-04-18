namespace PhotoFox.Model
{
    public class PhotoAlbum
    {
        public PhotoAlbum(
            string albumId, 
            string title, 
            string description, 
            string coverPhotoId)
        {
            this.AlbumId = albumId;
            this.Title = title;
            this.Description = description;
            this.CoverPhotoId = coverPhotoId;
        }

        public string AlbumId { get; }

        public string Title { get; }

        public string Description { get; }

        public string CoverPhotoId { get; }
    }
}
