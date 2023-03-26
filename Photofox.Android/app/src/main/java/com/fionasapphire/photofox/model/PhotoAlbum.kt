package com.fionasapphire.photofox.model

import com.fionasapphire.photofox.ImageReference

data class PhotoAlbum(
    val albumId: String,
    val title: String,
    val description: String,
    val image: ImageReference
)