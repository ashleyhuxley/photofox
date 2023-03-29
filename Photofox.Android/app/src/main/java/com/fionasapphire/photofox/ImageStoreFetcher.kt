package com.fionasapphire.photofox

import android.content.ContentValues.TAG
import android.content.Context
import android.util.Log
import coil.ImageLoader
import coil.decode.DataSource
import coil.decode.ImageSource
import coil.fetch.FetchResult
import coil.fetch.Fetcher
import coil.fetch.SourceResult
import coil.request.Options
import com.fionasapphire.photofox.model.ImageReference
import com.fionasapphire.photofox.storage.blob.ImageStorage
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import okio.*
import okio.Path.Companion.toPath
import java.io.ByteArrayInputStream
import java.io.File
import java.io.FileOutputStream

class ImageStoreFetcherFactory(private val context: Context): Fetcher.Factory<ImageReference> {
    override fun create(data: ImageReference, options: Options, imageLoader: ImageLoader): Fetcher? {
        return ImageStoreFetcher(data, options, context)
    }

}

class ImageStoreFetcher(
    private val image: ImageReference,
    private val options: Options,
    private val context: Context): Fetcher {

    private val connectionString = "DefaultEndpointsProtocol=https;AccountName=photofox;AccountKey=9HImTKLoDlh09Th4bo8xobaTXJe3mpPOASiVpnpLwsr5ox+QmnD7ZtMUaNnyqA0MKf99tkqYv3Zt+AStgHyEXw==;EndpointSuffix=core.windows.net"
    private val imageStorage: ImageStorage = ImageStorage(connectionString)

    override suspend fun fetch(): FetchResult? {

        val filename = "${context.externalCacheDir}/thumbs/${image.imageId}.jpg"

        val source: BufferedSource

        val file = File(filename)
        if (!file.exists()) {
            val bytes = withContext(Dispatchers.IO) {
                val bytes = imageStorage.getThumbnail(image.imageId)

                try {
                    file.createNewFile()
                    val fos = FileOutputStream(file)
                    fos.write(bytes)
                    fos.close()
                } catch (e: java.lang.Exception) {
                    if (e.message != null) {
                        Log.e(TAG, e.message!!)
                    }
                }

                bytes
            }

            source = ByteArrayInputStream(bytes).source().buffer()
        } else {
            source = FileSystem.SYSTEM.source(filename.toPath()).buffer()
        }

        return SourceResult(
            source = ImageSource(
                source = source,
                context = options.context,
            ),
            mimeType = null,
            dataSource = DataSource.MEMORY
        )
    }
}