Screen Logger for Unity 3D
Version 1.0
Giuseppe Portelli - giuseppe.portelli@gmail.com 

Requires Unity 4.5 or higher.

A simple and fully customizable screen logger. Just put it on a scene and you will see on screen output for each call to Debug.Log/LogWarning/LogError. 

Features
* Persistent
* Filter by message type
* Adjust font size
* Adjust color for each message type
* Adjust overlay size, anchoring and background color/opacity
* Toggle stack trace logging for each message type
* Toggle in editor visualization

Documentation
Screen logging is very useful when you are debugging a build of your game, for example when you are building for mobile or game consoles, or when you are testing things like screen resolution management which you cannot test directly in editor.

Screen Logger intercepts every call to Debug.Log / Debug.LogWarning / Debug.LogError and and displays each log message on a customizable screen overlay, you won't need to add extra logging instructions to your code, you will get on-screen the same output you have in editor. 

In order to add screen loggin to your game, open a scene and select from main menu: GameObject > Create Other > Screen Logger. A new ScreenLogger game object will be added to your scene. From this object you can customize all the logging features using the inspector. If you add the Screen Logger to the game's first scene and you keep the "Is Persistent" flag set to true, the object will persist over all the game scenes and you will always see logs on screen even when you load a new scene.

Screen Logger Properties
* Is Persistent - if set to true, the Screen Logger object will persist when you load a new scene
* Show in Editor - useful to preview the log overlay in editor
* Height - the height of the log overlay as a percentage of the screen height
* Width - the hwidth of the log overlay as a percentage of the screen width
* Margin - margin of the log overlay in pixels from the screen borders
* Anchor Position - top-left, top-right, bottom-left, bottom-right
* Font Size - log text font size
* Background Opacity - 0 is transparent, 1 is totally opaque
* Background Color - background color for the log overlay
* Log Messages - toggles log messages output
* Log Warnings - toggles warning messages output
* Log Errors - toggles errors/asserts/exceptions output
* Message Color - text color for log messages
* Warning Color - text color for warning messages
* Error Color - text color for error/assert/exception messages
* Stack Trace Messages - toggles stack trace log for messages
* Stack Trace Warnings - toggles stack trace log for warnings
* Stack Trace Errors - toggles stack trace log for errors/asserts/exceptions


