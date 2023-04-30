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
class FoldersViewModel
@Inject constructor(
    private val photoAlbumStorage: PhotoAlbumStorage
) : ViewModel() {
    val state = MutableStateFlow<FoldersViewModelState>(FoldersViewModelState.START)

    init {
        loadFolders()
    }

    private fun loadFolders() = viewModelScope.launch {
        state.value = FoldersViewModelState.LOADING
        try {
            val entities = withContext(Dispatchers.IO) { photoAlbumStorage.getPhotoAlbums() }
            val folders = entities.distinctBy { it.Folder }.map { it.Folder }
            state.value = FoldersViewModelState.SUCCESS(folders)
        } catch (e: Exception) {
            state.value = FoldersViewModelState.FAILURE(e.localizedMessage ?: "Unknown Error")
        }
    }
}