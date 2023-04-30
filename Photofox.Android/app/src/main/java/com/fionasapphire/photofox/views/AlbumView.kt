package com.fionasapphire.photofox.views

import android.annotation.SuppressLint
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.material.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.PlayArrow
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavHostController
import coil.compose.AsyncImage
import coil.request.ImageRequest
import com.fionasapphire.photofox.FailureView
import com.fionasapphire.photofox.ImageStoreFetcherFactory
import com.fionasapphire.photofox.LoadingView
import com.fionasapphire.photofox.R
import com.fionasapphire.photofox.model.PhotoAlbumEntry
import com.fionasapphire.photofox.viewmodels.PhotosViewModel
import com.fionasapphire.photofox.viewmodels.PhotosViewModelState

@Preview
@Composable
fun AlbumViewPreview() {

}

@Composable
fun AlbumView(navController: NavHostController) {
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
                albumId = viewModel.albumId,
                openImage = { viewModel.openImage(it, context) },
                albumName = successState.albumName,
                navController = navController
            )
        }
    }
}

@SuppressLint("UnusedMaterialScaffoldPaddingParameter")
@Composable
fun AlbumListView(
    photos: List<PhotoAlbumEntry>,
    albumId: String,
    albumName: String,
    openImage: (PhotoAlbumEntry) -> Unit,
    navController: NavHostController
) {
    val scaffoldState = rememberScaffoldState(rememberDrawerState(DrawerValue.Closed))
    val scope = rememberCoroutineScope()

    Scaffold(
        scaffoldState = scaffoldState,
        topBar = {
            TopAppBar(
                elevation = 4.dp,
                title = {
                    Text(albumName)
                },
                backgroundColor =  MaterialTheme.colors.primarySurface,
                actions = {
                    IconButton(onClick = { navController.navigate("videos/$albumId") }) {
                        Icon(Icons.Filled.PlayArrow, null)
                    }
                    IconButton(onClick = { navController.navigate("addPhotos/$albumId") }) {
                        Icon(Icons.Filled.Add, null)
                    }
//                    IconButton(onClick = { scope.launch { if(scaffoldState.drawerState.isClosed) scaffoldState.drawerState.open() else scaffoldState.drawerState.close() }}) {
//                        Icon(Icons.Filled.Menu, null)
//                    }
                })
        },
        drawerContent = {
            Column(modifier = Modifier.padding(10.dp)) {
                Text(text = "Add Photos...")
                Text(text = "Videos")
            }
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
            .clickable { openImage(item) },
        placeholder = painterResource(R.drawable.placeholder)
    )
}