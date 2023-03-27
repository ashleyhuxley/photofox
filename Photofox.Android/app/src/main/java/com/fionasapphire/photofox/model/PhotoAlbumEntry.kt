package com.fionasapphire.photofox.model

import com.fionasapphire.photofox.ImageReference
import java.time.Instant
import java.time.LocalDateTime

data class PhotoAlbumEntry (
    val photoId: String,
    val image: ImageReference,
    val date: Instant,
    val day: LocalDateTime
)