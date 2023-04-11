package com.fionasapphire.photofox.views

import android.annotation.SuppressLint
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.PickVisualMediaRequest
import androidx.activity.result.PickVisualMediaRequest.*
import androidx.activity.result.contract.ActivityResultContracts.*
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.ThumbUp
import androidx.compose.material.icons.filled.Warning
import androidx.compose.runtime.Composable
import androidx.compose.runtime.SideEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Alignment.Companion.CenterVertically
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.hilt.navigation.compose.hiltViewModel
import com.fionasapphire.photofox.viewmodels.AddPhotosViewModel
import com.fionasapphire.photofox.viewmodels.AddPhotosViewModelState
import com.fionasapphire.photofox.viewmodels.PhotoUploadState
import kotlinx.coroutines.flow.MutableStateFlow

@Composable
fun SelectPhotos() {
    val viewModel = hiltViewModel<AddPhotosViewModel>()
    val context = LocalContext.current



    val state by viewModel.state.collectAsState()
    when (state) {
        AddPhotosViewModelState.START -> {
            val pickMultipleMedia =
                rememberLauncherForActivityResult(PickMultipleVisualMedia(50)) { uris ->
                    viewModel.uploadAndQueue(uris, "00000000-0000-0000-0000-000000000000", context)
                }
            SideEffect {
                pickMultipleMedia.launch(PickVisualMediaRequest(PickVisualMedia.ImageAndVideo))
            }
        }
        is AddPhotosViewModelState.UPLOADING -> {
            PhotoUploadProgressView(states = (state as AddPhotosViewModelState.UPLOADING).states)
        }

    }
}

@SuppressLint("UnusedMaterialScaffoldPaddingParameter")
@Composable
fun PhotoUploadProgressView(states: List<MutableStateFlow<PhotoUploadState>>) {
    Scaffold(
        topBar = {
            TopAppBar(
                elevation = 4.dp,
                title = {
                    Text("Upload Progress")
                },
                backgroundColor = MaterialTheme.colors.primarySurface
            )
        },
    ) {
        LazyColumn(
            modifier = Modifier
                .fillMaxHeight()
                .fillMaxWidth()
        ) {
            items(items = states) { item ->
                Column(
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(5.dp),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    val state by item.collectAsState()
                    when (state) {
                        PhotoUploadState.UPLOADING -> {
                            ProgressRow(isInProgress = true, isError = false)
                        }
                        PhotoUploadState.SUCCESS -> {
                            ProgressRow(isInProgress = false, isError = false)
                        }
                        is PhotoUploadState.FAILURE -> {
                            ProgressRow(isInProgress = false, isError = true, errorText = (state as PhotoUploadState.FAILURE).error)
                        }
                    }
                }
            }
        }
    }
}

@Composable
fun ProgressRow(isInProgress: Boolean, isError: Boolean, errorText: String = "") {
    val text = if (isError) { errorText } else { if (isInProgress) { "Uploading..." } else { "Upload complete." } }

    Row(
        Modifier
            .fillMaxWidth()
            .height(60.dp)
            .padding(10.dp), verticalAlignment = CenterVertically
    ) {

        if (isInProgress) {
            CircularProgressIndicator()
        } else {
            if (isError) {
                Icon(Icons.Filled.Warning, null)
            } else {
                Icon(Icons.Filled.ThumbUp, null)
            }
        }
        
        Spacer(modifier = Modifier.width(20.dp))

        Text(
            text = text,
            fontSize = 20.sp,
            fontWeight = FontWeight.Bold,
            color = Color.Black,
        )
    }
}

@Preview(showSystemUi = true)
@Composable
fun Preview() {
    Column {
        ProgressRow(isInProgress = true, isError = false)
        ProgressRow(isInProgress = false, isError = false)
        ProgressRow(isInProgress = false, isError = true, errorText = "Upload failed")
    }
}