package com.fionasapphire.photofox.viewmodels

import com.fionasapphire.photofox.model.PhotoAlbum

sealed class PhotoAlbumsViewModelState {
    object START : PhotoAlbumsViewModelState()
    object LOADING : PhotoAlbumsViewModelState()
    data class SUCCESS(val albums: List<PhotoAlbum>) : PhotoAlbumsViewModelState()
    data class FAILURE(val message: String) : PhotoAlbumsViewModelState()
}