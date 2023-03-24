package com.fionasapphire.photofox

import com.fionasapphire.photofox.storage.PhotoAlbumStorage
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
    fun providePhotoAlbumStorage() = PhotoAlbumStorage("DefaultEndpointsProtocol=https;AccountName=photofox;AccountKey=9HImTKLoDlh09Th4bo8xobaTXJe3mpPOASiVpnpLwsr5ox+QmnD7ZtMUaNnyqA0MKf99tkqYv3Zt+AStgHyEXw==;EndpointSuffix=core.windows.net")
}