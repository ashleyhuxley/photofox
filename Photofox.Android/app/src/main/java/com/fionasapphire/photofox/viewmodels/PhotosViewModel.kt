package com.fionasapphire.photofox.viewmodels

import android.content.res.Resources.NotFoundException
import androidx.lifecycle.SavedStateHandle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.ImageReference
import com.fionasapphire.photofox.model.PhotoAlbumEntry
import com.fionasapphire.photofox.storage.table.PhotoAlbumStorage
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
    private val photoAlbumStorage: PhotoAlbumStorage,
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
            val album = withContext(Dispatchers.IO) { photoAlbumStorage.getAlbum(albumId) }
                ?: throw NotFoundException("Photo album with id $albumId not found")

            val entities = withContext(Dispatchers.IO) { photoInAlbumStorage.getPhotosInAlbum(albumId) }
            val photos = entities.map {
                PhotoAlbumEntry(it.rowKey, ImageReference(false, it.rowKey))
            }
            state.value = PhotosViewModelState.SUCCESS(photos, album.AlbumName)
        } catch (e: Exception) {
            state.value = PhotosViewModelState.FAILURE(e.localizedMessage ?: "Unknown Error")
        }
    }
}