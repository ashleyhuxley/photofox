package com.fionasapphire.photofox

import android.annotation.SuppressLint
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
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
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.modifier.modifierLocalConsumer
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.tooling.preview.PreviewParameter
import androidx.compose.ui.tooling.preview.PreviewParameterProvider
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import com.fionasapphire.photofox.ui.theme.PhotoFoxTheme
import dagger.hilt.android.AndroidEntryPoint
import kotlin.math.round

@AndroidEntryPoint
class MainActivity : ComponentActivity() {
    @SuppressLint("UnusedMaterialScaffoldPaddingParameter")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            PhotoFoxTheme {
                Scaffold(
                    topBar = {
                        TopAppBar(
                            title = { Text("PhotoFox") },
                            backgroundColor = Color.Black,
                            contentColor = Color.White
                        )
                    },
                ) {
                    MainView()
                }
            }
        }
    }


}

class SampleAlbumProvider: PreviewParameterProvider<List<PhotoAlbum>> {
    private val items = List(5) { index ->
        PhotoAlbum("1", "Album $index", "Test Album", null)
    }

    override val values = sequenceOf(items)
}

@Composable
@Preview(showBackground = true, showSystemUi = true)
fun Preview(@PreviewParameter(SampleAlbumProvider::class) items: List<PhotoAlbum>) {
    //FailureView(message = "Test message")
    //AlbumsListScreen(items.toList())
    LoadingView()
}

@Composable
fun MainView() {
    val viewModel = hiltViewModel<PhotoAlbumsViewModel>()
    val state by viewModel.state.collectAsState()
    when (state) {
        State.START -> {
        }
        State.LOADING -> {
            LoadingView()
        }
        is State.FAILURE -> {
            FailureView(message = (state as State.FAILURE).message)
        }
        is State.SUCCESS -> {
            val albums = (state as State.SUCCESS).albums
            AlbumsListScreen(albums)
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
fun LoadingView() {
    Box(contentAlignment = Alignment.Center, modifier = Modifier.fillMaxSize()) {
        Column (horizontalAlignment = Alignment.CenterHorizontally) {
            CircularProgressIndicator()
            Text(text = "Loading Albums...", modifier = Modifier.padding(20.dp))
        }
    }
}

@Composable
fun AlbumsListScreen(users: List<PhotoAlbum>) {
    LazyColumn(modifier = Modifier
        .fillMaxHeight()
        .fillMaxWidth()
            ) {
        items(items = users) { item ->
            Card(
                modifier = Modifier.fillMaxWidth().padding(10.dp),
                shape = RoundedCornerShape(10.dp),
                elevation = 5.dp
            ) {
                Column (modifier = Modifier.fillMaxWidth().padding(5.dp), horizontalAlignment = Alignment.CenterHorizontally) {
                    Box(modifier = Modifier.fillMaxWidth())
                    {
                        if (item.image == null) {
                            Image(
                                painterResource(id = R.drawable.placeholder),
                                "Placeholder",
                                contentScale = ContentScale.FillWidth,
                                modifier = Modifier.fillMaxWidth().padding(5.dp)
                            )
                        } else {
                            Image(
                                item.image.asImageBitmap(),
                                item.title,
                                contentScale = ContentScale.FillWidth,
                                modifier = Modifier.fillMaxWidth().padding(5.dp)
                            )
                        }
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