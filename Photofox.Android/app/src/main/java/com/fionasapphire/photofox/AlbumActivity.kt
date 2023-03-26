package com.fionasapphire.photofox

import android.annotation.SuppressLint
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import androidx.activity.compose.setContent
import androidx.compose.foundation.Image
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
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import com.fionasapphire.photofox.model.Photo
import com.fionasapphire.photofox.viewmodels.PhotosViewModel
import com.fionasapphire.photofox.viewmodels.PhotosViewModelState

class AlbumActivity : AppCompatActivity() {
    @SuppressLint("UnusedMaterialScaffoldPaddingParameter")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent{
/*            PhotoFoxTheme {
                Scaffold(
                    topBar = {
                        TopAppBar(
                            title = { Text("PhotoFox") },
                            backgroundColor = Color.Black,
                            contentColor = Color.White
                        )
                    },
                ) {
                    AlbumView()
                }
            }*/
        }
    }
}

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
            AlbumListView(photos = photos) {

            }
        }
    }


}

@OptIn(ExperimentalMaterialApi::class)
@Composable
fun AlbumListView(photos: List<Photo>, onHome: () -> Unit) {
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
                        if (item.image == null) {
                            Image(
                                painterResource(id = R.drawable.placeholder),
                                "Placeholder",
                                contentScale = ContentScale.FillWidth,
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(5.dp)
                            )
                        } else {
                            Image(
                                item.image.asImageBitmap(),
                                item.title,
                                contentScale = ContentScale.FillWidth,
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(5.dp)
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