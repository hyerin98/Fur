// using System;
// using System.Diagnostics;
// using System.Runtime.InteropServices;
// using IMFINE.Utils;
// using UnityEngine;


// void Awake()
// {
//     ConfigManager.instance.Prepared += ResizeWindow;
// }

// void Start()
// {
//     if (ConfigManager.instance.isPrepared) ResizeWindow();
// }

// bool isWindowResized;
// private void ResizeWindow()
// {
//     if (isWindowResized) return;

//     WindowManager.instance.Resize(ConfigManager.instance.data.screenPositionX,
//     ConfigManager.instance.data.screenPositionY,
//     ConfigManager.instance.data.screenWidth,
//     ConfigManager.instance.data.screenHeight);

//     isWindowResized = true;
// }