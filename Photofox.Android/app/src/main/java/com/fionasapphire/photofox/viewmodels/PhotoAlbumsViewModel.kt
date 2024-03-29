package com.fionasapphire.photofox.viewmodels

import androidx.lifecycle.SavedStateHandle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.model.ImageReference
import com.fionasapphire.photofox.model.PhotoAlbum
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
    private val savedStateHandle: SavedStateHandle
    ) : ViewModel() {

    val state = MutableStateFlow<PhotoAlbumsViewModelState>(PhotoAlbumsViewModelState.START)

    val folderId: String = savedStateHandle["folderId"] ?: ""
    init {
        loadAlbums()
    }

    /**
     * Load a list of available photo albums
     */
    private fun loadAlbums() = viewModelScope.launch {
        state.value = PhotoAlbumsViewModelState.LOADING
        try {
            val entities = withContext(Dispatchers.IO) { photoAlbumStorage.getPhotoAlbums(folderId) }
            val albums = entities
                .sortedBy { it.AlbumName }
                .map {
                    PhotoAlbum(it.partitionKey, it.AlbumName, it.AlbumDescription, ImageReference(true, it.CoverPhotoId), it.Folder)
                }
            state.value = PhotoAlbumsViewModelState.SUCCESS(albums)
        } catch (e: Exception) {
            state.value = PhotoAlbumsViewModelState.FAILURE(e.localizedMessage ?: "Unknown Error")
        }
    }

}