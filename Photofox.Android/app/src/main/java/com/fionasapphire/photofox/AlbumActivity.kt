package com.fionasapphire.photofox

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
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
import java.io.File


@Preview
@Composable
fun AlbumViewPreview() {

}

@Composable
fun AlbumView(onHome: () -> Unit, albumId: String?) {
    val viewModel = hiltViewModel<PhotosViewModel>()

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
            val photos = (state as PhotosViewModelState.SUCCESS).photos
            AlbumListView(
                photos = photos,
                onHome = {},
            )
        }
    }
}

@OptIn(ExperimentalMaterialApi::class)
@Composable
fun AlbumListView(photos: List<PhotoAlbumEntry>, onHome: () -> Unit) {
    val dir = LocalContext.current.externalCacheDir

    LazyColumn(modifier = Modifier
        .fillMaxHeight()
        .fillMaxWidth()
    ) {
        items(items = photos) { item ->
            Card(
                modifier = Modifier
                    .fillMaxWidth()
                    .padding(10.dp)
                    .clickable { },
                shape = RoundedCornerShape(10.dp),
                elevation = 5.dp,
                onClick = onHome
            ) {
                Column (modifier = Modifier
                    .fillMaxWidth()
                    .padding(5.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                    Box(modifier = Modifier.fillMaxWidth())
                    {
                        AsyncImage(
                            model = ImageRequest.Builder(LocalContext.current)
                                .data(File("$dir/thumbs/${item.image.imageId}.jpg"))
                                .crossfade(true)
                                .fetcherFactory(ImageStoreFetcherFactory())
                                .build(),
                            contentDescription = "",
                            contentScale = ContentScale.FillWidth,
                            modifier = Modifier.fillMaxWidth().padding(5.dp),
                            placeholder = painterResource(R.drawable.placeholder)
                        )
                    }
                }
            }
        }
    }
}