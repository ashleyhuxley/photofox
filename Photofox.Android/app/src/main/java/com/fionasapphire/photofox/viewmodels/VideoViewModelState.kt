package com.fionasapphire.photofox.viewmodels

import com.fionasapphire.photofox.model.AlbumVideoEntry

sealed class VideoViewModelState {
    object START : VideoViewModelState()
    object LOADING : VideoViewModelState()
    data class SUCCESS(val videos: List<AlbumVideoEntry>, val albumName: String) : VideoViewModelState()
    data class FAILURE(val message: String) : VideoViewModelState()
}