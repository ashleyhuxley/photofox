package com.fionasapphire.photofox

import android.app.Application
import android.content.Context
import dagger.hilt.android.HiltAndroidApp

@HiltAndroidApp
class PhotoFoxApplication: Application() {
    override fun onCreate() {
        super.onCreate()
    }

}
