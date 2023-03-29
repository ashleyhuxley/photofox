package com.fionasapphire.photofox.storage.queue

import com.microsoft.azure.storage.CloudStorageAccount
import com.microsoft.azure.storage.queue.CloudQueueMessage
import javax.inject.Inject


class QueueStorage
    @Inject constructor(private val connectionString: String) {
        fun enqueue(photoId: String, albumId: String) {
            val account = CloudStorageAccount.parse(connectionString)
            val queueClient = account.createCloudQueueClient()
            val queue = queueClient.getQueueReference("uploads")
            val message = CloudQueueMessage("$photoId,$albumId")
            queue.addMessage(message)
        }
}