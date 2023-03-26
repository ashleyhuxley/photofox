package com.fionasapphire.photofox.model

import android.graphics.Bitmap
import com.fionasapphire.photofox.ImageReference

data class Photo(
    val photoId: String,
    val title: String,
    val image: ImageReference
)