package com.fionasapphire.photofox.model

import android.graphics.Bitmap

data class Photo(
    val photoId: String,
    val title: String,
    val image: Bitmap?)