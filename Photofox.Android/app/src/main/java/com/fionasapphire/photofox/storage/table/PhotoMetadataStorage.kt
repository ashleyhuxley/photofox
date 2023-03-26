package com.fionasapphire.photofox.storage.table

import com.fionasapphire.photofox.storage.StorageBase
import com.fionasapphire.photofox.storage.entity.PhotoMetadataEntity
import com.fionasapphire.photofox.storage.enums.FieldName
import com.fionasapphire.photofox.storage.enums.TableName
import com.microsoft.azure.storage.table.TableQuery
import com.microsoft.azure.storage.table.TableQuery.QueryComparisons
import javax.inject.Inject

class PhotoMetadataStorage
    @Inject constructor(connectionString: String) : StorageBase(connectionString, TableName.PhotoMetadata.name) {

    fun getPhotoMetadata(photoId: String, partitionKey: String): PhotoMetadataEntity? {
        val table = getTableReference()

        val partitionKeyFilter = TableQuery.generateFilterCondition(
            FieldName.PartitionKey.name, QueryComparisons.EQUAL, partitionKey
        )

        val rowKeyFilter = TableQuery.generateFilterCondition(
            FieldName.RowKey.name, QueryComparisons.EQUAL, photoId
        )

        val filter = TableQuery.combineFilters(partitionKeyFilter, TableQuery.Operators.AND, rowKeyFilter)

        val query = TableQuery.from(PhotoMetadataEntity::class.java).where(filter)

        return table.execute(query).firstOrNull()
    }
}