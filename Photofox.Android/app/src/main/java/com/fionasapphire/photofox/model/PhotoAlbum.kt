package com.fionasapphire.photofox.model

/**
 * An album of photos in the main Photo Album list
 * @property albumId The Unique ID of the album
 * @property title The main display name
 * @property description The description of the album (currently unused)
 * @property image A reference to the image used as a cover photo
 */
data class PhotoAlbum(
    val albumId: String,
    val title: String,
    val description: String,
    val image: ImageReference
)