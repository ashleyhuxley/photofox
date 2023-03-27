package com.fionasapphire.photofox.viewmodels

import androidx.lifecycle.SavedStateHandle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.ImageReference
import com.fionasapphire.photofox.model.PhotoAlbumEntry
import com.fionasapphire.photofox.storage.table.PhotoInAlbumStorage
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
                PhotoAlbumEntry(it.rowKey, ImageReference(false, it.rowKey))
            }
            state.value = PhotosViewModelState.SUCCESS(photos)
        } catch (e: Exception) {
            state.value = PhotosViewModelState.FAILURE(e.localizedMessage)
        }
    }
}