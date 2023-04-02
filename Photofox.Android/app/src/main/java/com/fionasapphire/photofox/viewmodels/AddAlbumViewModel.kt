package com.fionasapphire.photofox.viewmodels

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.storage.table.PhotoAlbumStorage
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import javax.inject.Inject

@HiltViewModel
class AddAlbumViewModel
@Inject constructor(
    private val albumStorage: PhotoAlbumStorage
) : ViewModel() {

    fun addAlbum(albumName: String) = viewModelScope.launch {
        withContext(Dispatchers.IO) {
            albumStorage.addAlbum(albumName)
        }
    }

}