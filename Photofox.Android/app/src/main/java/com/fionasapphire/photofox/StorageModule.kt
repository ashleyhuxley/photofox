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
    @Provides
    @Singleton
    fun providePhotoAlbumStorage() = PhotoAlbumStorage(Config.connectionString)

    @Provides
    @Singleton
    fun provideImageStorage() = ImageStorage(Config.connectionString)

    @Provides
    @Singleton
    fun provideMetadataStorage() = PhotoMetadataStorage(Config.connectionString)

    @Provides
    @Singleton
    fun provideConnectionString() = Config.connectionString
}