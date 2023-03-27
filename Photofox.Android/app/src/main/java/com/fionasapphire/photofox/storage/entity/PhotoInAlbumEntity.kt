package com.fionasapphire.photofox.storage.entity

import com.microsoft.azure.storage.table.TableServiceEntity
import java.util.*

class PhotoInAlbumEntity : TableServiceEntity() {
    var UtcDate: Date = Date(0)
}