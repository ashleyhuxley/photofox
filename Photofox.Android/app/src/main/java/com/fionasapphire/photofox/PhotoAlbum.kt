package com.fionasapphire.photofox

class PhotoAlbum {
    constructor(albumId: String, title: String, description: String) {
        this.albumId = albumId
        this.title = title
        this.description = description
    }

    val albumId: String
    val title: String
    val description: String
}