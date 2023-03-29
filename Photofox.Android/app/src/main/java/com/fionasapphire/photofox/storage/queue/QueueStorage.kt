package com.fionasapphire.photofox.storage.queue

import com.microsoft.azure.storage.CloudStorageAccount
import com.microsoft.azure.storage.queue.CloudQueueMessage
import javax.inject.Inject


/**
 * A storage class for enqueuing messages
 */
class QueueStorage
    @Inject constructor(private val connectionString: String) {

    /**
     * Enqueue a message to process a newly uploaded photo
     * @param photoId The ID of the photo stored in the images container to process
     * @param albumId The ID of the album to which the image should be added
     */
    fun enqueue(photoId: String, albumId: String) {
            val account = CloudStorageAccount.parse(connectionString)
            val queueClient = account.createCloudQueueClient()
            val queue = queueClient.getQueueReference("uploads")
            val message = CloudQueueMessage("$photoId,$albumId")
            queue.addMessage(message)
        }
}