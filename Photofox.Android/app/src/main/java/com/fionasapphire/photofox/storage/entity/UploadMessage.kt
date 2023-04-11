package com.fionasapphire.photofox.storage.entity

import java.util.*

data class UploadMessage (
    val entityId: String,
    val type: String,
    val album: String,
    val title: String,
    val dateTaken: Date
)
