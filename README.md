# PhotoFox



## About this project

This is a personal project that started out as a simple way to store and access photos from Azure Storage. I have since evolved the project to include a web interface and an Adroid client alongside the original WPF desktop application. Photo uploads are processed using an Azure function.

I have since expanded on the project to include a variety of frameworks, technologies, patterns and practices which I have listed below.

## Microsoft Azure

### Azure Storage

The project stores photos and videos in Azure blob storage. Metadata about the photos (date taken, ISO, Aperture, Geolocation, etc) as well as data to sort the photos into albums is stored in Table Storage. Queues are used to process uploads.

### Azure Functions

There is an Azure function to process photo uploads. To prevent the need to do "heavy lifting" on the client (particularly the mobile app), the photos are uploaded to Azure storage and a message is enqueued which triggers an Azure function to process the photo. The function first hashes the photo to ensure duplicates are not uploaded, then extracts the metadata (EXIF) and generates a thumbnail.

## C# / .NET

### Async / Await

The project makes extensive use of Async/Await ensuring that network calls to Azure storage are done asynchronously which is reflected in the responsiveness of the UI.

### Unit Tests

The projects are covered by extensive unit tests which can be found in the corresponding Test projects. The tests use NUinit as a testing framework and Moq to mock dependencies.

### Extension Methods

A sample of extension methods can be found in PhotoFox.Core.Extensions

### WPF / MVVM

The desktop client is written in WPF using [.NET Community Toolkit (MVVM)](https://github.com/CommunityToolkit/dotnet).

### Blazor Web Application

This Blazor Client application provides a web interface to the photos and videos. The project uses ASP.NET Identity Services to allow for photo albums to be shared with friends & family through basic permissions. In a future update, I'd like to provide OAuth login for Google/Microsoft/etc. so that people do not need an account in order to participate.

The Google Maps API is used to display a collection of markers of where photos were taken, if they include geolocation metadata.

The application makes use of [IdentityAzureTable by David Melendez](https://elcamino.cloud/projects/docs/identityazuretable/) to store .NET Identity data in Azure storage so that a seperate SQL Database is not required for identity.

## Android

This is my first Android / Kotlin project, having been a C# developer for nearly all of my career. The project uses [Jetpack Compose](https://developer.android.com/jetpack/compose) for the UI, [Coil](https://coil-kt.github.io/coil/) to display images (with a custom Fetcher to fetch images from Azure storage) and [Hilt](https://developer.android.com/training/dependency-injection/hilt-android) for Dependency Injection.

Coroutines are used to run network-bound code on a different dispatcher. The app is fast and responsive, only displaying images as they are scrolled into view, meaning even large photo albums display almost instantly.