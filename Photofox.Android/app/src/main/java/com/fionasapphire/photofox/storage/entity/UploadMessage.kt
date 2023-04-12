package com.fionasapphire.photofox.storage.entity

import java.util.*

data class UploadMessage (
    val EntityId: String,
    val Type: String,
    val Album: String,
    val Title: String,
    val DateTaken: String,
    val FileExt: String
)
