package com.fionasapphire.photofox

import android.annotation.SuppressLint
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.tooling.preview.PreviewParameter
import androidx.compose.ui.tooling.preview.PreviewParameterProvider
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import coil.compose.AsyncImage
import coil.request.ImageRequest
import com.fionasapphire.photofox.model.ImageReference
import com.fionasapphire.photofox.model.PhotoAlbum
import com.fionasapphire.photofox.ui.theme.PhotoFoxTheme
import com.fionasapphire.photofox.viewmodels.PhotoAlbumsViewModel
import com.fionasapphire.photofox.viewmodels.PhotoAlbumsViewModelState
import com.fionasapphire.photofox.views.AddAlbumView
import com.fionasapphire.photofox.views.SelectPhotos
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContent {
            PhotoFoxTheme {
                Navi()
            }
        }
    }
}

class SampleAlbumProvider: PreviewParameterProvider<List<PhotoAlbum>> {
    private val items = List(5) { index ->
        PhotoAlbum("1", "Album $index", "Test Album", ImageReference(true, "test"))
    }

    override val values = sequenceOf(items)
}

@Composable
fun Navi() {
    val navController = rememberNavController()
    NavHost(navController = navController, startDestination = "home") {
        composable("home") {
            MainView(navController)
        }
        composable(
            route = "album/{albumId}",
            ) {
            AlbumView(navController)
        }
        composable("addAlbum") { AddAlbumView() }
        composable("addPhotos") { SelectPhotos() }
    }
}

@Composable
@Preview(showBackground = true, showSystemUi = true)
fun Preview(@PreviewParameter(SampleAlbumProvider::class) items: List<PhotoAlbum>) {
    //FailureView(message = "Test message")
    //AlbumsListScreen(items.toList())
    LoadingView("Things")
}

@Composable
fun MainView(navController: NavHostController) {
    val viewModel = hiltViewModel<PhotoAlbumsViewModel>()
    val state by viewModel.state.collectAsState()

    when (state) {
        PhotoAlbumsViewModelState.START -> {
        }
        PhotoAlbumsViewModelState.LOADING -> {
            LoadingView("Albums")
        }
        is PhotoAlbumsViewModelState.FAILURE -> {
            FailureView(message = (state as PhotoAlbumsViewModelState.FAILURE).message)
        }
        is PhotoAlbumsViewModelState.SUCCESS -> {
            val albums = (state as PhotoAlbumsViewModelState.SUCCESS).albums
            AlbumsListScreen(albums, navController)
        }
    }
}

@Composable
fun FailureView(message: String) {
    Column(modifier = Modifier.fillMaxSize()) {
        Text(text = "Something went wrong...", fontSize = 20.sp, modifier = Modifier.padding(10.dp))
        Text(text = message, modifier = Modifier.padding(10.dp))
    }
}

@Composable
fun LoadingView(entity: String) {
    Box(contentAlignment = Alignment.Center, modifier = Modifier.fillMaxSize()) {
        Column (horizontalAlignment = Alignment.CenterHorizontally) {
            CircularProgressIndicator()
            Text(text = "Loading ${entity}...", modifier = Modifier.padding(20.dp))
        }
    }
}

@SuppressLint("UnusedMaterialScaffoldPaddingParameter")
@OptIn(ExperimentalMaterialApi::class)
@Composable
fun AlbumsListScreen(albums: List<PhotoAlbum>, navController: NavHostController) {
    Scaffold(
        topBar = {
            TopAppBar(
                elevation = 4.dp,
                title = {
                    Text("PhotoFox")
                },
                backgroundColor =  MaterialTheme.colors.primarySurface,
                actions = {
                    IconButton(onClick = { navController.navigate("addAlbum") }) {
                        Icon(Icons.Filled.Add, null)
                    }
                })
        },
    ) {
        LazyColumn(
            modifier = Modifier
                .fillMaxHeight()
                .fillMaxWidth()
        ) {
            items(items = albums) { item ->
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(10.dp)
                        .clickable { },
                    shape = RoundedCornerShape(10.dp),
                    elevation = 5.dp,
                    onClick = { navController.navigate("album/${item.albumId}") }
                ) {
                    Column(
                        modifier = Modifier
                            .fillMaxWidth()
                            .padding(5.dp),
                        horizontalAlignment = Alignment.CenterHorizontally
                    ) {
                        Box(modifier = Modifier.fillMaxWidth())
                        {
                            AsyncImage(
                                model = ImageRequest.Builder(LocalContext.current)
                                    .data(item.image)
                                    .crossfade(true)
                                    .fetcherFactory(ImageStoreFetcherFactory(LocalContext.current))
                                    .build(),
                                contentDescription = item.title,
                                contentScale = ContentScale.FillWidth,
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(5.dp),
                                placeholder = painterResource(R.drawable.placeholder)
                            )
                        }
                        Text(
                            text = item.title,
                            fontSize = 20.sp,
                            fontWeight = FontWeight.Bold,
                            color = Color.Black,
                        )
                    }
                }
            }
        }
    }
}

