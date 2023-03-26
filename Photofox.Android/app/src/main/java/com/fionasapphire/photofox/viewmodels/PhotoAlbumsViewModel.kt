package com.fionasapphire.photofox.viewmodels

import android.graphics.Bitmap
import android.graphics.BitmapFactory
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.model.PhotoAlbum
import com.fionasapphire.photofox.storage.blob.ImageStorage
import com.fionasapphire.photofox.storage.table.PhotoAlbumStorage
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject

@HiltViewModel
class PhotoAlbumsViewModel
@Inject constructor(
    private val photoAlbumStorage: PhotoAlbumStorage,
    private val imageStorage: ImageStorage
    ) : ViewModel() {

    val state = MutableStateFlow<PhotoAlbumsViewModelState>(PhotoAlbumsViewModelState.START)

    init {
        loadAlbums()
    }

    private fun loadAlbums() = viewModelScope.launch {
        state.value = PhotoAlbumsViewModelState.LOADING
        try {
            val entities = withContext(Dispatchers.IO) { photoAlbumStorage.getPhotoAlbums() }
            val albums = entities
                .sortedBy { it.AlbumName }
                .map {
                    PhotoAlbum(it.partitionKey, it.AlbumName, it.AlbumDescription, getBitmap(it.CoverPhotoId))
                }
            state.value = PhotoAlbumsViewModelState.SUCCESS(albums)
        } catch (e: Exception) {
            state.value = PhotoAlbumsViewModelState.FAILURE(e.localizedMessage)
        }
    }

    private suspend fun getBitmap(photoId: String): Bitmap {
        val bytes = withContext(Dispatchers.IO) { imageStorage.getThumbnail(photoId) }
        return BitmapFactory.decodeByteArray(bytes, 0, bytes.size)
    }

}