package com.fionasapphire.photofox.storage.entity

import com.microsoft.azure.storage.table.TableServiceEntity
import java.util.*

class VideoInAlbumEntity : TableServiceEntity() {
    var Title: String = ""
    var VideoDate: Date = Date(0)
    var FileSize: Long = 0
    var FileExt: String = ""
}