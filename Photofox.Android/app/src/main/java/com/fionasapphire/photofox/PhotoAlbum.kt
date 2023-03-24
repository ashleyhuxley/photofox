package com.fionasapphire.photofox

import android.graphics.Bitmap

class PhotoAlbum {
    constructor(albumId: String, title: String, description: String, image: Bitmap) {
        this.albumId = albumId
        this.title = title
        this.description = description
        this.image = image
    }

    val albumId: String
    val title: String
    val description: String
    val image: Bitmap
}