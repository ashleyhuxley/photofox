package com.fionasapphire.photofox

import coil.ImageLoader
import coil.decode.DataSource
import coil.decode.ImageSource
import coil.fetch.FetchResult
import coil.fetch.Fetcher
import coil.fetch.SourceResult
import coil.request.Options
import com.fionasapphire.photofox.model.Photo
import com.fionasapphire.photofox.storage.blob.ImageStorage
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import okio.buffer
import okio.source
import java.io.ByteArrayInputStream

class ImageStoreFetcherFactory: Fetcher.Factory<ImageReference> {
    override fun create(data: ImageReference, options: Options, imageLoader: ImageLoader): Fetcher? {
        return ImageStoreFetcher(data, options)
    }

}

class ImageStoreFetcher(private val image: ImageReference, private val options: Options): Fetcher {
    private val connectionString = "DefaultEndpointsProtocol=https;AccountName=photofox;AccountKey=9HImTKLoDlh09Th4bo8xobaTXJe3mpPOASiVpnpLwsr5ox+QmnD7ZtMUaNnyqA0MKf99tkqYv3Zt+AStgHyEXw==;EndpointSuffix=core.windows.net"

    private val imageStorage: ImageStorage = ImageStorage(connectionString)

    override suspend fun fetch(): FetchResult? {

        val bytes = if (image.isThumbnail)
            withContext(Dispatchers.IO) {imageStorage.getThumbnail(image.imageId)
        } else {
            withContext(Dispatchers.IO) { imageStorage.getImage(image.imageId) }
        }

        return SourceResult(
            source = ImageSource(
                source = ByteArrayInputStream(bytes).source().buffer(),
                context = options.context,
            ),
            mimeType = null,
            dataSource = DataSource.MEMORY
        )
    }

}