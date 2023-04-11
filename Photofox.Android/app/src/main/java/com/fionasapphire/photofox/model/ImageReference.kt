package com.fionasapphire.photofox.model

/**
 * Represents an image in storage
 * @property isThumbnail If true, uses the Thumbnail data store instead of the main image store
 * @property imageId The unique ID of the image
 */
data class ImageReference (
    val isThumbnail: Boolean,
    val imageId: String
)