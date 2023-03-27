package com.fionasapphire.photofox.model

import com.fionasapphire.photofox.ImageReference

data class PhotoAlbumEntry (
    val photoId: String,
    val image: ImageReference
)