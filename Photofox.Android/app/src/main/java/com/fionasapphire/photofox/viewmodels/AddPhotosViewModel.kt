package com.fionasapphire.photofox.viewmodels

import android.content.Context
import android.net.Uri
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.storage.blob.ImageStorage
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
    private val imageStorage: ImageStorage,
    private val queueStorage: QueueStorage
) : ViewModel() {

    val state = MutableStateFlow<AddPhotosViewModelState>(AddPhotosViewModelState.START)

    init {

    }

    fun uploadAndQueue(uris: List<Uri>, albumId: String, context: Context) = viewModelScope.launch {

        val uploadStates = List<MutableStateFlow<PhotoUploadState>>(uris.size) {
            MutableStateFlow<PhotoUploadState>(
                PhotoUploadState.UPLOADING
            )
        }

        state.value = AddPhotosViewModelState.UPLOADING(uploadStates)
        for ((i, uri) in uris.withIndex()) {
            try {
                val photoId = UUID.randomUUID().toString()

                val iStream = context.contentResolver.openInputStream(uri)
                    ?: throw Exception("Unable to open file $uri")
                val bytes = getBytes(iStream)

                withContext(Dispatchers.IO) {
                    imageStorage.uploadImage(photoId, BlobType.images.name, bytes)
                    queueStorage.enqueue(photoId, albumId)
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