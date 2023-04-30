package com.fionasapphire.photofox.storage.entity

import com.microsoft.azure.storage.table.TableServiceEntity

class PhotoAlbumEntity : TableServiceEntity {
    var AlbumName: String = ""
    var AlbumDescription: String = ""
    var CoverPhotoId: String = ""
    var Folder: String = ""
    constructor() {

    }

    constructor(albumId : String, title: String) {
        rowKey = albumId
        AlbumName = title;
    }
}