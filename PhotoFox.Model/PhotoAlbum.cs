﻿namespace PhotoFox.Model
{
    public class PhotoAlbum
    {
        public PhotoAlbum(
            string albumId, 
            string title, 
            string description, 
            string coverPhotoId,
            string folder,
            string sortOrder,
            bool isPublic)
        {
            this.AlbumId = albumId;
            this.Title = title;
            this.Description = description;
            this.CoverPhotoId = coverPhotoId;
            this.Folder = folder;
            this.SortOrder = sortOrder;
            this.IsPublic = isPublic;
        }

        public string AlbumId { get; }
        public string Title { get; }
        public string Description { get; }
        public string CoverPhotoId { get; }
        public string Folder { get; }
        public string SortOrder { get; }
        public bool IsPublic { get; }
    }
}
