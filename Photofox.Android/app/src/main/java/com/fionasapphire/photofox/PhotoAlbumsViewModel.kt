package com.fionasapphire.photofox

import android.graphics.Bitmap
import android.graphics.BitmapFactory
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.storage.ImageStorage
import com.fionasapphire.photofox.storage.PhotoAlbumStorage
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

    val state = MutableStateFlow<State>(State.START)

    init {
        loadAlbums()
    }

    private fun loadAlbums() = viewModelScope.launch {
        state.value = State.LOADING
        try {
            val entities = withContext(Dispatchers.IO) { photoAlbumStorage.getPhotoAlbums() }
            val albums = entities.map { PhotoAlbum(it.AlbumId, it.AlbumName, it.AlbumDescription, getBitmap(it.CoverPhotoId)) }
            state.value = State.SUCCESS(albums)
        } catch (e: Exception) {
            state.value = State.FAILURE(e.localizedMessage)
        }
    }

    private suspend fun getBitmap(photoId: String): Bitmap {
        val bytes = withContext(Dispatchers.IO) { imageStorage.getThumbnail(photoId) }
        return BitmapFactory.decodeByteArray(bytes, 0, bytes.size)
    }

}