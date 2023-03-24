package com.fionasapphire.photofox.storage

import com.microsoft.azure.storage.CloudStorageAccount
import java.io.ByteArrayOutputStream

class ImageStorage
    constructor(private val connectionString: String)
{
    fun getThumbnail(imageId: String): ByteArray {
        val account = CloudStorageAccount.parse(connectionString)

        val client = account.createCloudBlobClient()
        val container = client.getContainerReference("thumbnails")

        val blob = container.getBlockBlobReference(imageId)
        val outputStream = ByteArrayOutputStream()

        blob.download(outputStream)

        return outputStream.toByteArray()
    }
}