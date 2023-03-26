package com.fionasapphire.photofox.viewmodels

import android.graphics.Bitmap
import android.graphics.BitmapFactory
import androidx.lifecycle.SavedStateHandle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.ImageReference
import com.fionasapphire.photofox.model.Photo
import com.fionasapphire.photofox.storage.blob.ImageStorage
import com.fionasapphire.photofox.storage.table.PhotoInAlbumStorage
import com.fionasapphire.photofox.storage.table.PhotoMetadataStorage
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject

@HiltViewModel
class PhotosViewModel
@Inject constructor(
    private val photoInAlbumStorage: PhotoInAlbumStorage,
    private val photoMetadataStorage: PhotoMetadataStorage,
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

    private fun loadPhotos(albumId: String)  = viewModelScope.launch {
        state.value = PhotosViewModelState.LOADING
        try {
            val entities = withContext(Dispatchers.IO) { photoInAlbumStorage.getPhotosInAlbum(albumId) }
            val photos = entities.map {
//                val metadata = withContext(Dispatchers.IO) { photoMetadataStorage.getPhotoMetadata(it.rowKey) }
//                if (metadata == null)
//                {
//                    state.value = PhotosViewModelState.FAILURE("Unable to find photo with ID ${it.rowKey}")
//                }

                Photo(it.rowKey, "Test", ImageReference(false, it.rowKey))
            }
            state.value = PhotosViewModelState.SUCCESS(photos)
        } catch (e: Exception) {
            state.value = PhotosViewModelState.FAILURE(e.localizedMessage)
        }
    }
}