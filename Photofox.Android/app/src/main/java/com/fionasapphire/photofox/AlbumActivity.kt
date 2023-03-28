package com.fionasapphire.photofox

import android.annotation.SuppressLint
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.material.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.hilt.navigation.compose.hiltViewModel
import coil.compose.AsyncImage
import coil.request.ImageRequest
import com.fionasapphire.photofox.model.PhotoAlbumEntry
import com.fionasapphire.photofox.viewmodels.PhotosViewModel
import com.fionasapphire.photofox.viewmodels.PhotosViewModelState

@Preview
@Composable
fun AlbumViewPreview() {

}

@Composable
fun AlbumView() {
    val viewModel = hiltViewModel<PhotosViewModel>()
    val context = LocalContext.current

    val state by viewModel.state.collectAsState()
    when (state) {
        PhotosViewModelState.START -> {
        }
        PhotosViewModelState.LOADING -> {
            LoadingView("Photos")
        }
        is PhotosViewModelState.FAILURE -> {
            FailureView(message = (state as PhotosViewModelState.FAILURE).message)
        }
        is PhotosViewModelState.SUCCESS -> {
            val successState = (state as PhotosViewModelState.SUCCESS)
            val photos = successState.photos
            AlbumListView(
                photos = photos.sortedByDescending { it.date },
                openImage = { viewModel.openImage(it, context) },
                albumName = successState.albumName
            )
        }
    }
}

@SuppressLint("UnusedMaterialScaffoldPaddingParameter")
@Composable
fun AlbumListView(
    photos: List<PhotoAlbumEntry>,
    albumName: String,
    openImage: (PhotoAlbumEntry) -> Unit) {

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(albumName) },
                backgroundColor = Color.Black,
                contentColor = Color.White
            )
        },
    ) {
        LazyVerticalGrid(
            columns = GridCells.Fixed(3),
            contentPadding = PaddingValues(
                start = 12.dp,
                top = 16.dp,
                end = 12.dp,
                bottom = 16.dp
            ),
            modifier = Modifier
                .fillMaxHeight()
                .fillMaxWidth()
        ) {
            items(photos.size) {
                    index -> ImageCard(photos[index], openImage)
            }
        }
    }
}

@Composable
fun ImageCard(item: PhotoAlbumEntry, openImage: (PhotoAlbumEntry) -> Unit) {
    AsyncImage(
        model = ImageRequest.Builder(LocalContext.current)
            .data(item.image)
            .crossfade(true)
            .fetcherFactory(ImageStoreFetcherFactory(LocalContext.current))
            .build(),
        contentDescription = "",
        contentScale = ContentScale.FillWidth,
        modifier = Modifier
            .fillMaxWidth()
            .padding(5.dp)
            .clickable{ openImage(item) },
        placeholder = painterResource(R.drawable.placeholder)
    )
}