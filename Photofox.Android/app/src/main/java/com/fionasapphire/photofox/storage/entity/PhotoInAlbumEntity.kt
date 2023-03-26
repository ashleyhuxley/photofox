package com.fionasapphire.photofox.storage.entity

import com.microsoft.azure.storage.table.TableServiceEntity
import java.time.ZoneId
import java.time.ZoneOffset
import java.time.format.DateTimeFormatter
import java.util.*

class PhotoInAlbumEntity : TableServiceEntity() {
    var UtcDate: Date = Date(0)

    fun getDateAsKey(): String {
        val newDate = UtcDate.toInstant()

        val df = DateTimeFormatter.ofPattern("yyyyMMdd").withZone(ZoneId.from(ZoneOffset.UTC))
        return df.format(newDate)
    }
}