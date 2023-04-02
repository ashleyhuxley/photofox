package com.fionasapphire.photofox.storage.table

import com.fionasapphire.photofox.storage.StorageBase
import com.fionasapphire.photofox.storage.entity.PhotoAlbumEntity
import com.fionasapphire.photofox.storage.enums.FieldName
import com.fionasapphire.photofox.storage.enums.TableName
import com.microsoft.azure.storage.table.TableOperation
import com.microsoft.azure.storage.table.TableQuery
import java.util.*
import javax.inject.Inject

class PhotoAlbumStorage
    @Inject constructor(connectionString: String) : StorageBase(connectionString, TableName.PhotoAlbums.name) {

    /**
     * Gets a list of available photo albums
     * @return A list of photo album entities
     */
    fun getPhotoAlbums(): List<PhotoAlbumEntity> {
        val table = getTableReference()

        val query = TableQuery.from(PhotoAlbumEntity::class.java)

        val res = table.execute(query)
        return res.toList()
    }

    /**
     * Gets an album by its unique ID
     * @param albumId THe unique ID of the album to return
     * @return The album represented by the specified ID, or null if not found
     */
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

    /**
     * Add a photo album to album storage
     * @param albumName The name of the album being added
     */
    fun addAlbum(albumName: String) {
        val albumId = UUID.randomUUID().toString()

        val entity = PhotoAlbumEntity()
        entity.partitionKey = albumId
        entity.rowKey = ""
        entity.AlbumName = albumName

        val table = getTableReference()

        val operation = TableOperation.insert(entity)
        table.execute(operation)
    }
}