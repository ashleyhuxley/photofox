package com.fionasapphire.photofox.viewmodels

import com.fionasapphire.photofox.model.PhotoAlbumEntry

sealed class PhotosViewModelState {
    object START : PhotosViewModelState()
    object LOADING : PhotosViewModelState()
    data class SUCCESS(val photos: List<PhotoAlbumEntry>) : PhotosViewModelState()
    data class FAILURE(val message: String) : PhotosViewModelState()
}