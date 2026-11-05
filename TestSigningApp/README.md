# Test Signing App - Minimal UWP Project

## Overview
This is a minimal UWP (Universal Windows Platform) application created for testing app signing after publish. It clones the manifest configuration from DRLMobile.Uwp but contains only basic functionality.

## Location
`C:\AppCodeStore\AI-Model-Code\TestSigningApp`

## Features
- ✅ Same Package.appxmanifest as DRLMobile.Uwp
- ✅ Same identity, publisher, and capabilities
- ✅ Same target platform versions (10.0.22621.0, min 10.0.19041.0)
- ✅ Same certificate thumbprint: `B37D10FECDC59494A238CE4A9874515D7AC142BA`
- ✅ Simple welcome page with no additional functionality
- ✅ Minimal codebase for easy testing

## Project Structure
```
TestSigningApp/
├── App.xaml                  # Application definition
├── App.xaml.cs              # Application logic
├── MainPage.xaml            # Welcome page UI
├── MainPage.xaml.cs         # Welcome page code-behind
├── Package.appxmanifest     # App manifest (cloned from DRLMobile.Uwp)
├── TestSigningApp.csproj    # Project file
├── Properties/
│   ├── AssemblyInfo.cs      # Assembly metadata
│   └── Default.rd.xml       # Runtime directives
└── Assets/                  # Required app assets (placeholder PNGs)
    ├── LockScreenLogo.scale-200.png
    ├── SplashScreen.scale-200.png
    ├── Square150x150Logo.scale-200.png
    ├── Square44x44Logo.scale-200.png
    ├── Square44x44Logo.targetsize-24_altform-unplated.png
    ├── Wide310x150Logo.scale-200.png
    └── StoreLogo.scale-200.png
```

## Manifest Details

### Identity
- **Name**: DRLEnterprises.HoneyforTest
- **Publisher**: CN=Drl Enterprises Inc, O=Drl Enterprises Inc, L=Glenview, S=Illinois, C=US
- **Version**: 1.1.13.0

### Capabilities
- internetClient
- internetClientServer
- picturesLibrary
- privateNetworkClientServer
- userNotificationListener
- systemManagement
- offlineMapsManagement
- chat
- bluetooth (device)
- location (device)
- pointOfService (device)

## Building the Project

### Prerequisites
- Visual Studio 2019 or later with UWP development workload
- Windows 10 SDK 10.0.22621.0 or later

### Build Steps
1. Open `TestSigningApp.csproj` in Visual Studio
2. Select configuration (Debug/Release) and platform (x86/x64/ARM/ARM64)
3. Build the solution (Ctrl+Shift+B)

### Publish for Testing
1. Right-click on the project in Solution Explorer
2. Select "Publish" > "Create App Packages"
3. Follow the wizard to create app packages
4. The package will be signed using the certificate thumbprint configured in the project

## Testing App Signing

This project is specifically designed to test the app signing process:

1. **Build the project** in Release mode
2. **Create app packages** through Visual Studio or MSBuild
3. **Verify the signature** of the generated .appx/.msix package
4. The package should be signed with the same certificate as DRLMobile.Uwp

## Differences from DRLMobile.Uwp

| Feature | DRLMobile.Uwp | TestSigningApp |
|---------|---------------|----------------|
| Pages | 40+ pages | 1 page (MainPage) |
| ViewModels | 30+ view models | None |
| Services | Multiple services | None |
| Dependencies | DevExpress, iText7, Microsoft.Toolkit | Only UWP Platform |
| Functionality | Full business app | Welcome message only |
| Purpose | Production app | Testing only |

## Notes

- All asset images are minimal 1x1 pixel placeholder PNGs
- No background tasks or extensions (removed from manifest)
- EntryPoint changed to `TestSigningApp.App`
- No external dependencies except Microsoft.NETCore.UniversalWindowsPlatform
- Safe to delete after testing

## Cleanup

To remove this test project:
```powershell
Remove-Item -Path "C:\AppCodeStore\AI-Model-Code\TestSigningApp" -Recurse -Force
```
