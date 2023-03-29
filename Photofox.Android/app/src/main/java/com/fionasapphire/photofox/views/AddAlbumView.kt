package com.fionasapphire.photofox.views

import androidx.compose.foundation.layout.*
import androidx.compose.material.Button
import androidx.compose.material.Text
import androidx.compose.material.TextField
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp

@Preview(showSystemUi = true)
@Composable
fun AddAlbumView() {
    Column(modifier = Modifier.padding(10.dp)) {
        TextField(modifier = Modifier.fillMaxWidth(), value = "New Album", onValueChange = {})
        Row (modifier = Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
            Button(onClick = { /*TODO*/ }) {
                Text("Add")
            }
            Button(onClick = { /*TODO*/ }) {
                Text("Cancel")
            }
        }
    }
}