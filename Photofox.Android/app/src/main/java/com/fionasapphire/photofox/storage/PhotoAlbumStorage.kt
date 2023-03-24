package com.fionasapphire.photofox.storage

import com.microsoft.azure.storage.CloudStorageAccount
import com.microsoft.azure.storage.StorageCredentialsAccountAndKey
import com.microsoft.azure.storage.table.TableQuery


class PhotoAlbumStorage
    constructor(private val connectionString: String)
{
    fun getPhotoAlbums(): List<PhotoAlbumEntity> {
        val account = CloudStorageAccount.parse(connectionString)

        val client = account.createCloudTableClient()
        val table = client.getTableReference("PhotoAlbums")
        table.createIfNotExists();
        val query = TableQuery.from(PhotoAlbumEntity::class.java)

        val res = table.execute(query)
        return res.toList()
    }
}