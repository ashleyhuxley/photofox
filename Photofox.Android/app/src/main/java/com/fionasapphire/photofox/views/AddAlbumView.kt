package com.fionasapphire.photofox.views

import androidx.compose.foundation.layout.*
import androidx.compose.material.Button
import androidx.compose.material.Text
import androidx.compose.material.TextField
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.TextFieldValue
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.hilt.navigation.compose.hiltViewModel
import com.fionasapphire.photofox.viewmodels.AddAlbumViewModel

@Preview(showSystemUi = true)
@Composable
fun PreviewAddAlbumView() {
    AddAlbumView { }
}


@Composable
fun AddAlbumView(onHome: () -> Unit) {
    var text by remember { mutableStateOf(TextFieldValue("")) }
    var viewModel = hiltViewModel<AddAlbumViewModel>()

    Column(modifier = Modifier.padding(10.dp)) {
        TextField(modifier = Modifier.fillMaxWidth(), value = text, onValueChange = { t -> text = t}, label = { Text("Album Name") })
        Row (modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.End) {
            Button(onClick = onHome) {
                Text("Cancel")
            }
            Spacer(Modifier.width(30.dp))
            Button(onClick = {
                viewModel.addAlbum(text.text)
                onHome()
                             }, enabled = text.text.isNotEmpty()) {
                Text("Add")
            }
        }
    }
}