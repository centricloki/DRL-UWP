# DRLMobile.UwpV2

## Overview
This is a UWP (Universal Windows Platform) application project that serves as V2 of the DRLMobile UWP application. It was created as an exact clone of the TestSigningApp project structure with updated namespaces and identifiers.

## Project Structure
- **App.xaml / App.xaml.cs**: Application entry point and lifecycle management
- **MainPage.xaml / MainPage.xaml.cs**: Main application page
- **Package.appxmanifest**: Application manifest with identity, capabilities, and configuration
- **Properties/**: Assembly information and runtime directives
- **Assets/**: Application images and icons

## Key Differences from TestSigningApp
- Namespace: `DRLMobile.UwpV2` (was `TestSigningApp`)
- Assembly Name: `DRLMobile.UwpV2` (was `TestSigningApp`)
- Package Identity: `DRLEnterprises.DRLMobileUwpV2` (was `DRLEnterprises.HoneyforTest`)
- Display Name: `DRLMobile UwpV2` (was `Honey for Test`)
- Entry Point: `DRLMobile.UwpV2.App` (was `TestSigningApp.App`)
- Project GUID: `{B2C3D4E5-F6A7-8901-BCDE-F12345678901}`

## Capabilities
The application includes the following capabilities:
- Internet Client & Server
- Pictures Library
- Private Network Client Server
- User Notification Listener
- System Management (IoT)
- Offline Maps Management
- Chat
- Bluetooth
- Location
- Point of Service

## Building the Project
1. Open `DRLMobileSolution.sln` in Visual Studio
2. Select the desired platform (x86, x64, ARM, ARM64)
3. Build the solution

## Target Platform
- Target Platform Version: 10.0.22621.0
- Minimum Platform Version: 10.0.19041.0
- .NET Core Universal Windows Platform: 6.2.14

## Notes
- This project maintains the same manifest configuration and capabilities as TestSigningApp
- All asset files have been copied from TestSigningApp
- The project is ready for development and customization
