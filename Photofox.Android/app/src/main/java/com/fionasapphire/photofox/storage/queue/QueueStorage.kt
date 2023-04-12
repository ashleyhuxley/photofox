package com.fionasapphire.photofox.storage.queue

import com.fionasapphire.photofox.storage.entity.UploadMessage
import com.google.gson.Gson
import com.microsoft.azure.storage.CloudStorageAccount
import com.microsoft.azure.storage.queue.CloudQueueMessage
import java.text.DateFormat
import java.text.SimpleDateFormat
import java.util.*
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
     * @param title The title of the uploaded image
     * @param dateTaken A fallback date to be used if date taken cannot be extracted from EXIF
     */
    fun enqueue(photoId: String, albumId: String, title: String, dateTaken: Date) {

            val df: DateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm'Z'", Locale.ENGLISH)
            df.timeZone = TimeZone.getTimeZone("UTC")

            val gson = Gson()
            val msg = UploadMessage(photoId, "PHOTO", albumId, title, df.format(dateTaken), "JPG")

            val account = CloudStorageAccount.parse(connectionString)
            val queueClient = account.createCloudQueueClient()
            val queue = queueClient.getQueueReference("uploads")

            val message = CloudQueueMessage(gson.toJson(msg).toByteArray())

            queue.addMessage(message)
        }
}