package com.fionasapphire.photofox.viewmodels

sealed class FoldersViewModelState {
    object START : FoldersViewModelState()
    object LOADING : FoldersViewModelState()
    data class SUCCESS(val folders: List<String>) : FoldersViewModelState()
    data class FAILURE(val message: String) : FoldersViewModelState()
}