package com.fionasapphire.photofox

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.storage.PhotoAlbumStorage
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject

@HiltViewModel
class PhotoAlbumsViewModel
@Inject constructor(private val photoAlbumStorage: PhotoAlbumStorage) : ViewModel() {

    val state = MutableStateFlow<State>(State.START)

    init {
        loadAlbums()
    }

    private fun loadAlbums() = viewModelScope.launch {
        state.value = State.LOADING
        try {
            val albums = withContext(Dispatchers.IO) { photoAlbumStorage.getPhotoAlbums() }
            state.value = State.SUCCESS(albums.map {
                PhotoAlbum(it.AlbumId, it.AlbumName, it.AlbumDescription )
            })
        } catch (e: Exception) {
            state.value = State.FAILURE(e.localizedMessage)
        }
    }

}