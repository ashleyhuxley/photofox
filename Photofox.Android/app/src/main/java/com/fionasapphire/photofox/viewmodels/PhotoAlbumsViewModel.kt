package com.fionasapphire.photofox.viewmodels

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.ImageReference
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
    private val photoAlbumStorage: PhotoAlbumStorage
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
                    PhotoAlbum(it.partitionKey, it.AlbumName, it.AlbumDescription, ImageReference(true, it.CoverPhotoId))
                }
            state.value = PhotoAlbumsViewModelState.SUCCESS(albums)
        } catch (e: Exception) {
            state.value = PhotoAlbumsViewModelState.FAILURE(e.localizedMessage)
        }
    }

}