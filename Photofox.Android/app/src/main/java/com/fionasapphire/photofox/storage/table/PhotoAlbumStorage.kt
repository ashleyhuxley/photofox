package com.fionasapphire.photofox.storage.table

import com.fionasapphire.photofox.storage.StorageBase
import com.fionasapphire.photofox.storage.entity.PhotoAlbumEntity
import com.fionasapphire.photofox.storage.entity.PhotoMetadataEntity
import com.fionasapphire.photofox.storage.enums.FieldName
import com.fionasapphire.photofox.storage.enums.TableName
import com.microsoft.azure.storage.table.TableQuery
import javax.inject.Inject

class PhotoAlbumStorage
    @Inject constructor(connectionString: String) : StorageBase(connectionString, TableName.PhotoAlbums.name) {
    fun getPhotoAlbums(): List<PhotoAlbumEntity> {
        val table = getTableReference()

        val query = TableQuery.from(PhotoAlbumEntity::class.java)

        val res = table.execute(query)
        return res.toList()
    }

    fun getAlbum(albumId: String): PhotoAlbumEntity? {
        val table = getTableReference()

        val partitionKeyFilter = TableQuery.generateFilterCondition(
            FieldName.PartitionKey.name, TableQuery.QueryComparisons.EQUAL, albumId
        )

        val rowKeyFilter = TableQuery.generateFilterCondition(
            FieldName.RowKey.name, TableQuery.QueryComparisons.EQUAL, ""
        )

        val filter = TableQuery.combineFilters(partitionKeyFilter, TableQuery.Operators.AND, rowKeyFilter)

        val query = TableQuery.from(PhotoAlbumEntity::class.java).where(filter)

        return table.execute(query).firstOrNull()
    }
}