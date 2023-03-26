package com.fionasapphire.photofox.viewmodels

import com.fionasapphire.photofox.model.Photo

sealed class PhotosViewModelState {
    object START : PhotosViewModelState()
    object LOADING : PhotosViewModelState()
    data class SUCCESS(val photos: List<Photo>) : PhotosViewModelState()
    data class FAILURE(val message: String) : PhotosViewModelState()
}