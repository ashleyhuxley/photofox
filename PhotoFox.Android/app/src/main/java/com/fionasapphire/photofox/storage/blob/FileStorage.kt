package com.fionasapphire.photofox.storage.blob

import com.fionasapphire.photofox.storage.enums.BlobType
import com.microsoft.azure.storage.CloudStorageAccount
import java.io.ByteArrayOutputStream

/**
 * Stores and retrieves images and thumbnails from Azure storage
 */
class FileStorage
    constructor(private val connectionString: String)
{
    /**
     * Uploads an image to Azure storage in the specified container
     * @param id A unique ID that represents the file
     * @param containerName The name of the container in which the file will be stored
     * @param data A byte array representing the file data
     */
    fun uploadFile(id: String, containerName: String, data: ByteArray) {
        val account = CloudStorageAccount.parse(connectionString)

        val client = account.createCloudBlobClient()
        val container = client.getContainerReference(containerName)

        val blob = container.getBlockBlobReference(id)

        blob.uploadFromByteArray(data, 0, data.size)
    }

    /**
     * Gets the thumbnail of an image from the thumbnails container
     * @param imageId The ID of the image for which the thumbnail will be retrieved
     */
    fun getThumbnail(imageId: String): ByteArray {
        return getResource(imageId, BlobType.thumbnails.name)
    }

    /**
     * Gets an image from the images container
     * @param imageId The ID of the image to be retrieved
     */
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