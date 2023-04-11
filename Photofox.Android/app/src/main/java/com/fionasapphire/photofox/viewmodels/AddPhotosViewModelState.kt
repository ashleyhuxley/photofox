package com.fionasapphire.photofox.viewmodels

import kotlinx.coroutines.flow.MutableStateFlow

sealed class PhotoUploadState {
    object UPLOADING: PhotoUploadState()
    object SUCCESS: PhotoUploadState()
    data class FAILURE(val error: String): PhotoUploadState()
}

sealed class AddPhotosViewModelState {
    object START : AddPhotosViewModelState()
    data class UPLOADING(val states: List<MutableStateFlow<PhotoUploadState>>) : AddPhotosViewModelState()
}