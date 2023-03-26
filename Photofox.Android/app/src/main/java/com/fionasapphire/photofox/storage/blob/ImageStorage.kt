package com.fionasapphire.photofox.storage.blob

import com.microsoft.azure.storage.CloudStorageAccount
import java.io.ByteArrayOutputStream

class ImageStorage
    constructor(private val connectionString: String)
{
    fun getThumbnail(imageId: String): ByteArray {
        return getResource(imageId, "thumbnails")
    }

    fun getImage(imageId: String): ByteArray {
        return getResource(imageId, "images")
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