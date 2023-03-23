package com.fionasapphire.photofox

sealed class State {
    object START : State()
    object LOADING : State()
    data class SUCCESS(val albums: List<PhotoAlbum>) : State()
    data class FAILURE(val message: String) : State()
}