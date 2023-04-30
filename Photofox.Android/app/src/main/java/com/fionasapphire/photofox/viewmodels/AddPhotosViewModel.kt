package com.fionasapphire.photofox.viewmodels

import android.content.Context
import android.net.Uri
import androidx.lifecycle.SavedStateHandle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.storage.blob.FileStorage
import com.fionasapphire.photofox.storage.enums.BlobType
import com.fionasapphire.photofox.storage.queue.QueueStorage
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.*
import java.util.*
import javax.inject.Inject


@HiltViewModel
class AddPhotosViewModel
@Inject constructor(
    private val fileStorage: FileStorage,
    private val queueStorage: QueueStorage,
    private val savedStateHandle: SavedStateHandle
) : ViewModel() {

    val state = MutableStateFlow<AddPhotosViewModelState>(AddPhotosViewModelState.START)
    val albumId: String? = savedStateHandle["albumId"]

    /**
     * Upload a list of photos to a particular album
     * @param uris A list of content URIs containing the photos to be uploaded
     * @param albumId The ID of the album in which to place the photos
     * @param context An Android Context used to resolve the URIs
     */
    fun uploadAndQueue(uris: List<Uri>, albumId: String, context: Context) = viewModelScope.launch {

        val uploadStates = List(uris.size) {
            MutableStateFlow<PhotoUploadState>(
                PhotoUploadState.UPLOADING
            )
        }

        state.value = AddPhotosViewModelState.UPLOADING(uploadStates)
        for ((i, uri) in uris.withIndex()) {
            try {
                // New photo ID is a random GUID
                val photoId = UUID.randomUUID().toString()

                // Get image contents
                val iStream = context.contentResolver.openInputStream(uri)
                    ?: throw Exception("Unable to open file $uri")
                val bytes = getBytes(iStream)

                // Extract filename
                val name = uri.toString().substring(uri.toString().lastIndexOf('/'))

                val date = Date()

                // Upload the content to storage and queue a message for it to be processed
                withContext(Dispatchers.IO) {
                    fileStorage.uploadFile(photoId, BlobType.uploads.name, bytes)
                    queueStorage.enqueue(photoId, albumId, name, date)

                    uploadStates[i].value = PhotoUploadState.SUCCESS
                }
            }
            catch (e: Exception) {
                uploadStates[i].value = PhotoUploadState.FAILURE(e.localizedMessage ?: "Unknown Error")
            }

        }
    }

    @Throws(IOException::class)
    fun getBytes(inputStream: InputStream): ByteArray {
        val byteBuffer = ByteArrayOutputStream()
        val bufferSize = 1024
        val buffer = ByteArray(bufferSize)
        var len = 0
        while (inputStream.read(buffer).also { len = it } != -1) {
            byteBuffer.write(buffer, 0, len)
        }
        return byteBuffer.toByteArray()
    }
}