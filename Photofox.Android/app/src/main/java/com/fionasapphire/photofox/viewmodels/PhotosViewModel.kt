package com.fionasapphire.photofox.viewmodels

import android.content.ContentValues
import android.content.Context
import android.content.Intent
import android.content.Intent.FLAG_ACTIVITY_NEW_TASK
import android.content.Intent.FLAG_GRANT_READ_URI_PERMISSION
import android.content.res.Resources.NotFoundException
import android.os.Environment
import android.util.Log
import androidx.activity.result.PickVisualMediaRequest
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.content.FileProvider
import androidx.lifecycle.SavedStateHandle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.ImageReference
import com.fionasapphire.photofox.PhotoFoxApplication
import com.fionasapphire.photofox.atMidnight
import com.fionasapphire.photofox.model.PhotoAlbumEntry
import com.fionasapphire.photofox.storage.blob.ImageStorage
import com.fionasapphire.photofox.storage.table.PhotoAlbumStorage
import com.fionasapphire.photofox.storage.table.PhotoInAlbumStorage
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.File
import java.io.FileOutputStream
import javax.inject.Inject


@HiltViewModel
class PhotosViewModel
@Inject constructor(
    private val photoInAlbumStorage: PhotoInAlbumStorage,
    private val photoAlbumStorage: PhotoAlbumStorage,
    private val imageStorage: ImageStorage,
    private val savedStateHandle: SavedStateHandle
    ): ViewModel() {
    val state = MutableStateFlow<PhotosViewModelState>(PhotosViewModelState.START)

    init {
        val albumId: String? = savedStateHandle["albumId"]
        if (albumId == null)
        {
            state.value = PhotosViewModelState.FAILURE("No album ID specified")
        }

        loadPhotos(albumId = albumId!!)
    }


    fun openImage(photo: PhotoAlbumEntry, context: Context) {
        viewModelScope.launch {
            val filename = "${context.getExternalFilesDir(Environment.DIRECTORY_PICTURES)}/${photo.image.imageId}.jpg"
            val file = File(filename)

            withContext(Dispatchers.IO) {
                val bytes = imageStorage.getImage(photo.image.imageId)

                try {
                    file.createNewFile()
                    val fos = FileOutputStream(file)
                    fos.write(bytes)
                    fos.close()
                } catch (e: java.lang.Exception) {
                    if (e.message != null) {
                        Log.e(ContentValues.TAG, e.message!!)
                    }
                }
            }

            val intent = Intent(Intent.ACTION_VIEW)
            val uri = FileProvider.getUriForFile(
                context,
                context.applicationContext.packageName + ".provider",
                file
            )
            intent.setDataAndType(uri, "image/jpeg")
            intent.addFlags(FLAG_ACTIVITY_NEW_TASK)
            intent.addFlags(FLAG_GRANT_READ_URI_PERMISSION)
            context.startActivity(intent)
        }
    }

    private fun loadPhotos(albumId: String)  = viewModelScope.launch {
        state.value = PhotosViewModelState.LOADING
        try {
            val album = withContext(Dispatchers.IO) { photoAlbumStorage.getAlbum(albumId) }
                ?: throw NotFoundException("Photo album with id $albumId not found")

            val entities = withContext(Dispatchers.IO) { photoInAlbumStorage.getPhotosInAlbum(albumId) }
            val photos = entities.map {
                PhotoAlbumEntry(
                    it.rowKey,
                    ImageReference(true, it.rowKey),
                    it.UtcDate.toInstant(),
                    it.UtcDate.atMidnight()
                )
            }
            state.value = PhotosViewModelState.SUCCESS(photos, album.AlbumName)
        } catch (e: Exception) {
            state.value = PhotosViewModelState.FAILURE(e.localizedMessage ?: "Unknown Error")
        }
    }
}