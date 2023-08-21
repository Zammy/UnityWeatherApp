# UnityWeatherApp

## Toasty Package

Android specific logic for showing a "toast" message is implemented in a separate package. Package consists of only one static method `Toast.DisplayToastMessage(string text)`. There is no point to test Android SDK methods so only one test was implemented to test message queueing logic used. To achieve that both `AndroidJavaObject` and `AndroidJavaClass`  had to be wrapped in our own classes so that they can become testable as unit-tests in Editor scripting inside Unity. A mocking library might have helped to achieve this "cleaner" but I preferred to write it myself as it was only one test.

## Weather App

App is architecture is done using "anti-pattern": **Service Locator**. Although it is considered anti-pattern, compared to dependency injection, I believe it is a better fit for small and medium sized apps/games in Unity.
Except `App.cs` which acts as a controller and a view for the application, I didn't see a point separating them as requirements are tiny, there are three services abstracted behind interfaces. `LocationService` which deals with getting user location. `WeatherService` which deals with getting current weather reports based on location. Adding unit-tests to application logic will be quite easy when services are abstracted behind interfaces that are easily mockable with a mocking library.
Last service is `CoroutineRunnerService`. A utility service to abstract coroutine starting and stopping that will definitely be needed if we are to write tests for the app logic.

## Build

[Here](build.apk) you can download the APK build for Android. I do not own an iPhone at the moment so I have not implemented the iOS version of Toasty package.
