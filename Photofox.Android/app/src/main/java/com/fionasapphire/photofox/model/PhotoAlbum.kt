package com.fionasapphire.photofox.model

import android.graphics.Bitmap

data class PhotoAlbum(
    val albumId: String,
    val title: String,
    val description: String,
    val image: Bitmap?)