package com.fionasapphire.photofox.storage.table

import com.fionasapphire.photofox.storage.StorageBase
import com.fionasapphire.photofox.storage.entity.PhotoAlbumEntity
import com.fionasapphire.photofox.storage.entity.PhotoInAlbumEntity
import com.fionasapphire.photofox.storage.enums.FieldName
import com.fionasapphire.photofox.storage.enums.TableName
import com.microsoft.azure.storage.table.TableOperation
import com.microsoft.azure.storage.table.TableQuery
import java.util.*
import javax.inject.Inject

class PhotoInAlbumStorage
    @Inject constructor(connectionString: String) : StorageBase(connectionString, TableName.PhotoInAlbum.name) {

    /**
     * Get a list of photos in a particular album
     * @param albumId The ID of the album from which to return photos
     * @return A list of PhotoInAlbum entities that are in the specified album
     */
        fun getPhotosInAlbum(albumId: String): List<PhotoInAlbumEntity> {
            val table = getTableReference()

            val filter = TableQuery.generateFilterCondition(
                FieldName.PartitionKey.name, TableQuery.QueryComparisons.EQUAL, albumId
            )

            val query = TableQuery.from(PhotoInAlbumEntity::class.java).where(filter)

            val res = table.execute(query)

            return res.toList()
        }
}