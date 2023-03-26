package com.fionasapphire.photofox

import com.fionasapphire.photofox.storage.blob.ImageStorage
import com.fionasapphire.photofox.storage.table.PhotoAlbumStorage
import com.fionasapphire.photofox.storage.table.PhotoMetadataStorage
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent
import javax.inject.Singleton

@Module
@InstallIn(SingletonComponent::class)
object  StorageModule {

    private const val connectionString = "DefaultEndpointsProtocol=https;AccountName=photofox;AccountKey=9HImTKLoDlh09Th4bo8xobaTXJe3mpPOASiVpnpLwsr5ox+QmnD7ZtMUaNnyqA0MKf99tkqYv3Zt+AStgHyEXw==;EndpointSuffix=core.windows.net"

    @Provides
    fun provideConnectionString(): String {
        return connectionString
    }

    @Provides
    @Singleton
    fun providePhotoAlbumStorage() = PhotoAlbumStorage(connectionString)

    @Provides
    @Singleton
    fun provideImageStorage() = ImageStorage(connectionString)

    @Provides
    @Singleton
    fun provideMetadataStorage() = PhotoMetadataStorage(connectionString)
}