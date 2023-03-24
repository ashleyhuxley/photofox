package com.fionasapphire.photofox

import android.annotation.SuppressLint
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.material.*
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import com.fionasapphire.photofox.ui.theme.PhotoFoxTheme
import dagger.hilt.android.AndroidEntryPoint

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

@Composable
@Preview(showBackground = true)
fun Preview() {
    //FailureView(message = "Test message")
    val items = List(5) { index ->
        PhotoAlbum("1", "Album $index", "Test Album")
    }

    AlbumsListScreen(items)
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
        CircularProgressIndicator()
    }
}

@Composable
fun AlbumsListScreen(users: List<PhotoAlbum>) {
    LazyColumn(modifier = Modifier.fillMaxHeight()) {
        items(items = users) { item ->
            Row(modifier = Modifier.padding(8.dp),
                verticalAlignment = Alignment.CenterVertically) {

                Box(modifier = Modifier
                    .background(Color.Black, CircleShape)
                    .size(50.dp),
                    contentAlignment = Alignment.Center ){
                    Text(
                        text = item.title.substring(0, 1),
                        color = Color.White,
                        fontSize = 26.sp,
                        fontWeight = FontWeight.Bold)
                }
                Column(
                    modifier = Modifier
                        .align(Alignment.CenterVertically)
                        .padding(start = 6.dp)
                ) {
                    Text(
                        text = item.title,
                        fontSize = 20.sp,
                        fontWeight = FontWeight.Bold,
                        color = Color.Black)
                    Text(
                        text = item.description, fontSize = 16.sp,
                        color = Color.Black,
                        modifier = Modifier.padding(top = 2.dp))
                }
            }
        }
    }
}