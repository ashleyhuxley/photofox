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
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.fionasapphire.photofox.ui.theme.PhotoFoxTheme
import com.fionasapphire.photofox.viewmodels.FoldersViewModel
import com.fionasapphire.photofox.viewmodels.FoldersViewModelState
import com.fionasapphire.photofox.views.*
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
@Composable
fun Navi() {
    val navController = rememberNavController()
    NavHost(navController = navController, startDestination = "home") {
        composable("home") {
            MainView(navController)
        }
        composable("album/{albumId}") {
            AlbumView(navController)
        }
        composable("folder/{folderId}") {
            AlbumsList(navController)
        }
        composable("videos/{albumId}") {
            VideosView(navController)
        }
        composable("addAlbum") {
            AddAlbumView(onHome = { navController.popBackStack() })
        }
        composable("addPhotos/{albumId}") {
            SelectPhotos(navController)
        }
    }
}
@OptIn(ExperimentalMaterialApi::class)
@Composable
fun MainView(navController: NavHostController) {
    val viewModel = hiltViewModel<FoldersViewModel>()
    val state by viewModel.state.collectAsState()

    when (state) {
        FoldersViewModelState.START -> {
        }
        FoldersViewModelState.LOADING -> {
            LoadingView("Albums")
        }
        is FoldersViewModelState.FAILURE -> {
            FailureView(message = (state as FoldersViewModelState.FAILURE).message)
        }
        is FoldersViewModelState.SUCCESS -> {
            val folders = (state as FoldersViewModelState.SUCCESS).folders
            FoldersListScreen(folders, navController)
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
@Composable
@ExperimentalMaterialApi
fun FoldersListScreen(folders: List<String>, navController: NavHostController) {
    Scaffold(
        topBar = {
            TopAppBar(
                elevation = 4.dp,
                title = {
                    Text("PhotoFox")
                },
                backgroundColor = MaterialTheme.colors.primarySurface
            )
        }
    ) {
        LazyColumn(
            modifier = Modifier
                .fillMaxWidth()
                .fillMaxHeight()
        )
        {
            items(items = folders) {
                Card(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(10.dp)
                        .clickable { },
                    shape = RoundedCornerShape(10.dp),
                    elevation = 5.dp,
                    onClick = { navController.navigate("folder/${it}") }
                ) {
                    Text(
                        modifier = Modifier.padding(10.dp),
                        text = it,
                        fontSize = 20.sp,
                        fontWeight = FontWeight.Bold,
                        color = Color.Black,
                    )
                }
            }
        }
    }
}

