package com.fionasapphire.photofox.storage.table

import com.fionasapphire.photofox.storage.StorageBase
import com.fionasapphire.photofox.storage.entity.PhotoInAlbumEntity
import com.fionasapphire.photofox.storage.entity.VideoInAlbumEntity
import com.fionasapphire.photofox.storage.enums.FieldName
import com.fionasapphire.photofox.storage.enums.TableName
import com.microsoft.azure.storage.table.TableQuery
import javax.inject.Inject

class VideoInAlbumStorage
@Inject constructor(connectionString: String) : StorageBase(connectionString, TableName.VideoInAlbum.name) {
    fun getVideosInAlbum(albumId: String): List<VideoInAlbumEntity> {
        val table = getTableReference()

        val filter = TableQuery.generateFilterCondition(
            FieldName.PartitionKey.name, TableQuery.QueryComparisons.EQUAL, albumId
        )

        val query = TableQuery.from(VideoInAlbumEntity::class.java).where(filter)

        val res = table.execute(query)

        return res.toList()
    }
}