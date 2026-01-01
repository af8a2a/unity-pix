# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity PIX is a Windows-only integration plugin that enables GPU captures from the Unity Editor using Microsoft's PIX on Windows tool. It requires Windows 10+ and DirectX 12.

## Build Commands

### Native Plugin (C++)

Build the native DLL using CMake with MSVC and Ninja:

```bash
cd UnityPix
cmake -B build -G Ninja -DCMAKE_CXX_COMPILER=cl -DCMAKE_BUILD_TYPE=Release
cmake --build build
```

The built DLL is automatically copied to `Packages/com.deltation.unity-pix/Plugins/UnityPix/x64/`.

### Unity Package

The package is located at `Packages/com.deltation.unity-pix/` and requires Unity 2021.3+. It's editor-only code in the `DELTation.UnityPix.Editor` namespace.

## Architecture

### Native Plugin (UnityPix/)

- **main.cpp**: Core Unity plugin implementing the low-level PIX integration
  - `UnityPluginLoad`: Loads WinPixGpuCapturer.dll when `-unity-pix-capture` command line arg is present
  - `IsPixLoaded`: Checks if PIX is attached
  - `PixGpuCaptureNextFrames`: Triggers GPU capture for N frames
  - `OpenPixCapture`: Opens capture file in PIX UI
  - Uses Unity's native plugin interface (IUnityGraphics, IUnityLog)

### Unity Editor Package (Packages/com.deltation.unity-pix/Editor/)

- **UnityPixBindings.cs**: P/Invoke declarations for native plugin functions
- **CaptureUtility.cs**: High-level capture orchestration with async/await pattern using GraphicsFence for GPU synchronization
- **ICaptureTarget.cs**: Interface for capture targets (defines `RenderAsync()` and `NumFrames`)
- **GameViewCaptureTarget.cs** / **SceneViewCaptureTarget.cs**: Capture target implementations
- **GameViewCaptureTool.cs**: Menu item at `Tools/Unity PIX/Capture Game View`
- **UnityPixToolbar.cs**: Scene view overlay toolbar button
- **UnityPixPreferences.cs**: User preferences under `Preferences/Unity PIX`

### Key Integration Points

1. PIX attaches via command line `-unity-pix-capture` flag or by launching Unity through PIX UI
2. Captures are saved to `Assets/../UnityPixCaptures/` (configurable) with `.wpix` extension
3. Capture files are automatically git-ignored via generated `.gitignore`
