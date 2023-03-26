package com.fionasapphire.photofox.storage.entity

import com.microsoft.azure.storage.table.TableServiceEntity
import java.util.*

class PhotoMetadataEntity : TableServiceEntity() {
    var PhotoId: String = ""
    var Aperture: String = ""
    var Device: String = ""
    var DimensionHeight: Int = 0
    var DimensionWidth: Int = 0
    var Exposure: String = ""
    var FileHash: String = ""
    var FileSize: Int = 0
    var FocalLength: String = ""
    var ISO: String = ""
    var Manufacturer: String = ""
    var Orientation: Int = 0
    var Title: String = ""
    var UtcDate: Date = Date(0)
}