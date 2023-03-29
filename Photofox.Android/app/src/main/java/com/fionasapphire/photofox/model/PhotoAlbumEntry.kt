package com.fionasapphire.photofox.model

import java.time.Instant
import java.time.LocalDateTime

/**
 * Represents a photo being part of an album
 * @property photoId The unique ID of the photo
 * @property image A reference to the image data for this photo
 * @property date The date and time this photo was taken
 * @property day The day (without time) that this photo was taken. Used for grouping.
 */
data class PhotoAlbumEntry (
    val photoId: String,
    val image: ImageReference,
    val date: Instant,
    val day: LocalDateTime
)