package com.fionasapphire.photofox.storage

import com.microsoft.azure.storage.table.TableServiceEntity

class PhotoAlbumEntity : TableServiceEntity {
    var AlbumId: String = ""
    var AlbumName: String = ""
    var AlbumDescription: String = ""
    var CoverPhotoId: String = ""

    constructor() {

    }

    constructor(albumId : String, title: String) {
        AlbumId = albumId
        AlbumName = title;
    }
}