package com.fionasapphire.photofox.viewmodels

import android.content.ContentValues
import android.content.Context
import android.content.Intent
import android.content.res.Resources
import android.os.Environment
import android.util.Log
import androidx.compose.ui.text.toUpperCase
import androidx.core.content.FileProvider
import androidx.lifecycle.SavedStateHandle
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.fionasapphire.photofox.model.AlbumVideoEntry
import com.fionasapphire.photofox.model.PhotoAlbumEntry
import com.fionasapphire.photofox.storage.blob.FileStorage
import com.fionasapphire.photofox.storage.table.PhotoAlbumStorage
import com.fionasapphire.photofox.storage.table.VideoInAlbumStorage
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.File
import java.io.FileOutputStream
import javax.inject.Inject

@HiltViewModel
class VideosViewModel
@Inject constructor(
    private val videoInAlbumStorage: VideoInAlbumStorage,
    private val photoAlbumStorage: PhotoAlbumStorage,
    private val fileStorage: FileStorage,
    private val savedStateHandle: SavedStateHandle
) : ViewModel() {

    val state = MutableStateFlow<VideoViewModelState>(VideoViewModelState.START)

    init {
        val albumId: String? = savedStateHandle["albumId"]
        if (albumId == null)
        {
            state.value = VideoViewModelState.FAILURE("No album ID specified")
        }

        loadVideos(albumId = albumId!!)
    }

    private fun loadVideos(albumId: String)  = viewModelScope.launch {
        state.value = VideoViewModelState.LOADING
        try {
            val album = withContext(Dispatchers.IO) { photoAlbumStorage.getAlbum(albumId) }
                ?: throw Resources.NotFoundException("Photo album with id $albumId not found")

            val entities = withContext(Dispatchers.IO) { videoInAlbumStorage.getVideosInAlbum(albumId) }
            val videos = entities.map {
                AlbumVideoEntry(
                    it.rowKey,
                    it.Title,
                    it.VideoDate,
                    it.FileExt
                )
            }
            state.value = VideoViewModelState.SUCCESS(videos, album.AlbumName)
        } catch (e: Exception) {
            state.value = VideoViewModelState.FAILURE(e.localizedMessage ?: "Unknown Error")
        }
    }

    fun openVideo(video: AlbumVideoEntry, context: Context) {
        viewModelScope.launch {
            val filename = "${context.getExternalFilesDir(Environment.DIRECTORY_PICTURES)}/${video.videoId}.jpg"

            // Download the video file from storage
            withContext(Dispatchers.IO) {
                fileStorage.downloadVideo(video.videoId, filename)
            }

            // Start a new Action to open the image
            val intent = Intent(Intent.ACTION_VIEW)
            val file = File(filename)
            val uri = FileProvider.getUriForFile(
                context,
                context.applicationContext.packageName + ".provider",
                file
            )
            intent.setDataAndType(uri, getMimeType(video.extension))
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
            intent.addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION)
            context.startActivity(intent)
        }
    }

    private fun getMimeType(ext: String) : String {
        return when (ext.uppercase()) {
            "MP4" -> "video/mp4"
            "AVI" -> "video/x-msvideo"
            "MKV" -> "video/x-matroska"
            "M4V" -> "video/x-m4v"
            else -> {
                throw java.lang.IndexOutOfBoundsException("Unknown video type: $ext")
            }
        }
    }
}