package com.fionasapphire.photofox.views

import android.annotation.SuppressLint
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavHostController
import com.fionasapphire.photofox.FailureView
import com.fionasapphire.photofox.LoadingView
import com.fionasapphire.photofox.model.AlbumVideoEntry
import com.fionasapphire.photofox.viewmodels.VideoViewModelState
import com.fionasapphire.photofox.viewmodels.VideosViewModel

@Composable
fun VideosView(navController: NavHostController) {
    val viewModel = hiltViewModel<VideosViewModel>()
    val state by viewModel.state.collectAsState()
    val context = LocalContext.current

    when (state) {
        VideoViewModelState.START -> {

        }
        VideoViewModelState.LOADING -> {
            LoadingView("Videos")
        }
        is VideoViewModelState.FAILURE -> {
            FailureView(message = (state as VideoViewModelState.FAILURE).message)
        }
        is VideoViewModelState.SUCCESS -> {
            val successState = (state as VideoViewModelState.SUCCESS)
            val videos = successState.videos
            AlbumVideosView(
                videos = videos.sortedByDescending { it.date },
                openVideo = { viewModel.openVideo(it, context) },
                albumName = successState.albumName,
                navController = navController
            )
        }
    }
}

@Composable
@SuppressLint("UnusedMaterialScaffoldPaddingParameter")
fun AlbumVideosView(
    videos: List<AlbumVideoEntry>,
    openVideo: (AlbumVideoEntry) -> Unit,
    albumName: String,
    navController: NavHostController
    ) {
    Scaffold(
        topBar = {
            TopAppBar(
                elevation = 4.dp,
                title = {
                    Text(albumName)
                },
                backgroundColor =  MaterialTheme.colors.primarySurface,
                actions = {

                })
    }) {
        LazyColumn {
            items(items = videos) { item ->
                Text(
                    modifier =
                    Modifier
                        .padding(12.dp)
                        .clickable { openVideo(item) },
                    text = item.title,
                    fontSize = 18.sp,
                    fontWeight = FontWeight.Bold
                )
            }
        }
    }
}