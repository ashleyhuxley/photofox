package com.fionasapphire.photofox.storage.blob

import com.fionasapphire.photofox.storage.enums.BlobType
import com.microsoft.azure.storage.CloudStorageAccount
import java.io.ByteArrayOutputStream

class ImageStorage
    constructor(private val connectionString: String)
{
    fun uploadImage(imageId: String, containerName: String, data: ByteArray) {
        val account = CloudStorageAccount.parse(connectionString)

        val client = account.createCloudBlobClient()
        val container = client.getContainerReference(containerName)

        val blob = container.getBlockBlobReference(imageId)

        blob.uploadFromByteArray(data, 0, data.size)
    }

    fun getThumbnail(imageId: String): ByteArray {
        return getResource(imageId, BlobType.thumbnails.name)
    }

    fun getImage(imageId: String): ByteArray {
        return getResource(imageId, BlobType.images.name)
    }

    private fun getResource(imageId: String, containerName: String) : ByteArray {
        val account = CloudStorageAccount.parse(connectionString)

        val client = account.createCloudBlobClient()
        val container = client.getContainerReference(containerName)

        val blob = container.getBlockBlobReference(imageId)
        val outputStream = ByteArrayOutputStream()

        blob.download(outputStream)

        return outputStream.toByteArray()
    }
}