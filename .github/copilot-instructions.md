# Snap Hutao
Snap Hutao (胡桃工具箱) is an open-source Genshin Impact toolkit under MIT license, designed for modern Windows platforms. It's a .NET 9.0 WinUI application that provides desktop tools for Genshin Impact players without requiring mobile devices.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Branch Workflow Requirements

**CRITICAL**: Always work on the `develop` branch for all development work:
- **Always checkout and work on `develop` branch**: `git checkout develop`
- **Always target PRs to `develop` branch** - never target `main` or other branches
- **All feature development, bug fixes, and contributions must be done on `develop`**
- The `main` branch is reserved for stable releases only

## Working Effectively

### Prerequisites - Install on Windows Only
- Install [Visual Studio 2022 Community](https://visualstudio.microsoft.com/downloads/)
  - Visual Studio will automatically install required workloads when opening the solution
  - For Visual Studio 2022 17.8: Install [Single-project MSIX Packaging Tools for VS 2022](https://marketplace.visualstudio.com/items?itemName=ProjectReunion.MicrosoftSingleProjectMSIXPackagingToolsDev17)
- Install [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Ensure Windows SDK 10.0.26100.0 or later is installed (bundled with Visual Studio)

### Build and Test Process
- Clone repository: `git clone https://github.com/DGP-Studio/Snap.Hutao.git`
- Switch to `develop` branch: `git checkout develop`
- Navigate to repository root
- Install Cake build tool: `dotnet tool restore` -- takes 30 seconds. NEVER CANCEL.
- Restore NuGet packages: `dotnet restore src/Snap.Hutao/Snap.Hutao.sln --configfile NuGet.Config` -- takes 20 seconds. NEVER CANCEL.
- **CRITICAL: Full build requires Windows environment. Linux/macOS builds will fail.**
- Run tests only: `dotnet test src/Snap.Hutao/Snap.Hutao.Test/Snap.Hutao.Test.csproj` -- takes 10 seconds. Expect 5-10 test failures on non-Windows platforms.

### Build on Windows (Complete Process)
- **WARNING: Build process takes 45+ minutes. NEVER CANCEL. Set timeout to 90+ minutes.**
- Full build: `dotnet tool restore && dotnet cake` -- takes 45+ minutes. NEVER CANCEL.
- Build creates MSIX package in `src/output/` directory
- Build requires Windows SDK and MSIX packaging tools
- **MANUAL VALIDATION REQUIRED**: After building, install and test the MSIX package by launching the application and verifying core functionality

### Development Workflow
- **REMINDER: Always work on `develop` branch and target PRs to `develop`** (see Branch Workflow Requirements above)
- Main project solution: `src/Snap.Hutao/Snap.Hutao.sln`
- Main application project: `src/Snap.Hutao/Snap.Hutao/Snap.Hutao.csproj`
- Test project: `src/Snap.Hutao/Snap.Hutao.Test/Snap.Hutao.Test.csproj`
- Open solution in Visual Studio 2022 for best development experience

## Validation Requirements

### Testing
- Run unit tests: `dotnet test src/Snap.Hutao/Snap.Hutao.Test/Snap.Hutao.Test.csproj` -- takes 8-15 seconds
- Test success criteria: 47+ out of 52 tests should pass
- **Expected test failures on non-Windows**: Registry access, network connectivity, file path issues
- **Always test on Windows before submitting PRs**

### Manual Validation Scenarios
- **CRITICAL**: After any UI changes, build and install the MSIX package locally
- **Test core workflows**: 
  - Application launch and initialization
  - User interface responsiveness
  - Game data synchronization features
  - Settings configuration and persistence
- **Always validate changes by running the actual application**, not just building

### Code Quality Requirements
- **No linting tools configured** - rely on Visual Studio and compiler warnings
- **No automated formatting** - follow existing code style
- **Always run full test suite before committing**
- Use existing StyleCop and analyzer configurations in project files

## Coding Standards

### File Headers and Licensing
- All C# files must include MIT license header:
  ```csharp
  // Copyright (c) DGP Studio. All rights reserved.
  // Licensed under the MIT license.
  ```

### Namespace and Type Conventions
- **File-scoped namespaces**: Use `namespace Snap.Hutao.Extension;` format
- **PascalCase**: All types, properties, methods, and public members
- **Interface naming**: Interfaces must start with "I" (e.g., `IEntityAccess`)
- **No var keyword**: Use explicit type declarations (enforced as warning)
  ```csharp
  // ✅ Correct
  string value = GetValue();
  List<string> items = new();
  
  // ❌ Avoid
  var value = GetValue();
  var items = new List<string>();
  ```

### Code Style and Formatting
- **Indentation**: 4 spaces, no tabs
- **Line endings**: CRLF (Windows style)
- **Braces**: Always use braces for control structures
- **Using directives**: Place outside namespace, order System first
- **Expression-bodied members**: Prefer for single-line accessors only
- **Modern C# features**: Use pattern matching, records, and latest language features

### Architecture Patterns
- **MVVM**: Strict Model-View-ViewModel separation
- **Dependency Injection**: Use built-in .NET DI container with `[Injection]` attributes
- **Async/Await**: Prefer async methods, use `ConfigureAwait(false)` in libraries
- **Extension methods**: Organize by type in separate static classes
- **Global usings**: Common namespaces defined in `GlobalUsing.cs`

### Documentation and Comments
- **XML documentation**: Not required for internal APIs
- **Inline comments**: Minimal, code should be self-documenting
- **TODO/HACK comments**: Avoid in production code

### Error Handling
- **Exceptions**: Use specific exception types, avoid generic `Exception`
- **Nullable reference types**: Enabled throughout project
- **Defensive programming**: Validate parameters and handle edge cases

## UI Guidelines and XAML Standards

### XAML Architecture
- **WinUI 3**: Modern Windows UI framework with Material Design principles
- **MVVM Pattern**: Strict separation between View (XAML) and ViewModel (C#)
- **Page-based Navigation**: Each major feature has its own Page in `UI/Xaml/View/Page/`
- **Custom Controls**: Application-specific controls in `shuxc` namespace
- **Behaviors**: UI logic encapsulated in `shuxb` namespace behaviors

### XAML Formatting (XAML Styler)
- **Attribute ordering**: Standardized order with `x:Class`, `xmlns`, positioning, sizing
- **Indentation**: Clean hierarchical structure with proper spacing
- **Line breaks**: Keep related attributes grouped, break for readability
- **Element structure**:
  ```xml
  <Grid x:Name="MainGrid"
        Margin="16"
        HorizontalAlignment="Stretch">
      <!-- Content -->
  </Grid>
  ```

### Namespace Conventions
- **Standard namespaces**: Use conventional WinUI namespace prefixes
  - Default: `http://schemas.microsoft.com/winfx/2006/xaml/presentation`
  - `x:` for XAML features
  - `cw:` for CommunityToolkit.WinUI
  - `cwc:` for CommunityToolkit.WinUI.Controls
- **Custom namespaces**: Snap Hutao specific
  - `shuxc:` for custom controls (`using:Snap.Hutao.UI.Xaml.Control`)
  - `shuxb:` for behaviors (`using:Snap.Hutao.UI.Xaml.Behavior`)
  - `shuxm:` for markup extensions (`using:Snap.Hutao.UI.Xaml.Markup`)

### Page and Control Structure
- **ScopedPage**: Use `shuxc:ScopedPage` as base for application pages
- **Design-time data**: Include `d:DataContext` for ViewModel binding
- **Background themes**: Use `{ThemeResource ApplicationPageBackgroundThemeBrush}`
- **Accessibility**: Include appropriate automation properties
- **Resource management**: Leverage theme resources and styles

### Data Binding Patterns
- **ViewModel binding**: Use `{Binding PropertyName}` for ViewModel properties
- **Command binding**: Use `{Binding CommandName}` for ICommand implementations
- **Design-time context**: `d:DataContext="{d:DesignInstance Type}"`
- **Converters**: Place data converters in `UI/Xaml/Data/` folder

### Visual Design Standards
- **Theming**: Support Light/Dark themes with appropriate resources
- **Layout**: Use Grid, StackPanel, and modern layout containers
- **Spacing**: Consistent margins and padding using theme resources
- **Cards**: Use `AcrylicGridCardStyle` and `GridCardStyle` for content grouping
- **Icons**: Leverage Segoe Fluent Icons and custom application icons

### Responsive Design
- **Adaptive layouts**: Design for various window sizes and orientations
- **Content scaling**: Use relative sizing and theme-appropriate dimensions
- **Navigation**: Implement proper focus management and keyboard navigation

## Common Build Issues and Solutions

### Network/Firewall Limitations
- CommunityToolkit-Labs NuGet feed may fail in restricted environments
- **Workaround**: Use cached packages or work from a less restricted network
- Document in instructions: "NuGet restore may fail due to firewall limitations on CommunityToolkit-Labs packages"

### Platform-Specific Limitations
- **Cannot build on Linux/macOS** - Windows SDK and MSIX tooling required
- **Registry and Windows-specific APIs** - tests will fail on non-Windows platforms
- **Always include platform warnings** in any build instructions

## Project Structure

### Repository Root
- `src/Snap.Hutao/Snap.Hutao/` - Main application project
- `src/Snap.Hutao/Snap.Hutao.Test/` - Unit test project  
- `src/Snap.Hutao/Snap.Hutao.Benchmark/` - Performance benchmarks
- `.github/workflows/` - CI/CD pipeline definitions
- `build.cake` - Cake build script for automated builds
- `NuGet.Config` - NuGet package source configuration

### Main Application Structure (`src/Snap.Hutao/Snap.Hutao/`)
- `Core/` - Core application functionality
  - `Abstraction/` - Abstract base classes and interfaces
  - `Caching/` - Memory and disk caching systems
  - `Database/` - Entity Framework and database operations
  - `DependencyInjection/` - IoC container configuration
  - `Diagnostics/` - Application diagnostics and monitoring
  - `IO/` - File system operations and utilities
  - `Threading/` - Async/await patterns and thread safety
- `UI/` - User interface components and utilities
  - `Input/` - Keyboard and mouse input handling
  - `Shell/` - Application shell and navigation
  - `Windowing/` - Window management and display
  - `Xaml/` - XAML controls, behaviors, and utilities
    - `View/` - XAML pages and user controls
      - `Page/` - Application pages (Settings, Game, etc.)
      - `Dialog/` - Modal dialogs and popups
      - `Card/` - Reusable card components
    - `Control/` - Custom WinUI controls
    - `Behavior/` - XAML behaviors for UI interactions
    - `Data/` - Data binding converters and templates
- `ViewModel/` - MVVM view models organized by feature
  - `Achievement/` - Achievement tracking
  - `Game/` - Game launch and management
  - `Setting/` - Application settings and preferences
  - `User/` - User account and authentication
  - `Calendar/` - Event calendar functionality
- `Service/` - Business logic and external integrations
  - `Game/` - Genshin Impact game integration
  - `Metadata/` - Game data and asset management
  - `Navigation/` - Page navigation and routing
  - `Notification/` - Toast and in-app notifications
  - `Update/` - Application update management
- `Model/` - Data models and entities
  - `Entity/` - Database entity models
  - `Metadata/` - Game metadata structures
  - `InterChange/` - Data exchange formats (UIGF, etc.)
  - `Primitive/` - Basic value types and structures
- `Extension/` - Extension methods for built-in and external types
- `Web/` - HTTP clients and web service integrations
- `Win32/` - Windows API interop and native functionality

### Important Files
- `Package.appxmanifest` - MSIX application manifest
- `Package.development.appxmanifest` - Development manifest
- `GlobalUsing.cs` - Global using directives for common namespaces
- `stylecop.json` - StyleCop analyzer configuration
- `.editorconfig` - Code formatting and style rules
- `settings.xamlstyler` - XAML formatting configuration
- `.filenesting.json` - Visual Studio file nesting rules

## CI/CD Information

### GitHub Actions Workflows
- **Alpha workflow**: Triggered on `develop` branch changes
- **Canary workflow**: Triggered on feature branches (merges all open PRs)
- **Build time**: 45+ minutes including package signing. NEVER CANCEL.
- **Artifacts**: Signed MSIX packages uploaded as GitHub releases

### Build Process Overview
1. NuGet package restoration (20 seconds)
2. AppX manifest generation 
3. .NET compilation and linking (30 minutes)
4. Asset and resource copying
5. MSIX package creation (10 minutes)
6. Code signing (5 minutes)

## Troubleshooting

### Common Issues
- **"Registry is not supported on this platform"** - Expected on non-Windows
- **"Name or service not known"** - Network/firewall blocking external APIs  
- **"UnsafeAccessor missing field"** - Platform-specific .NET behavior differences
- **"Cannot find file"** - Windows vs Unix path separator issues

### Recovery Steps
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Clean build artifacts: Delete `src/Snap.Hutao/Snap.Hutao/bin/` and `src/Snap.Hutao/Snap.Hutao/obj/`
- Reset tools: `dotnet tool restore --force-evaluation`

## Time Expectations
- **NEVER CANCEL**: Full build takes 45+ minutes. Set timeout to 90+ minutes.
- **NEVER CANCEL**: Test suite takes 5-10 seconds but may timeout on slow systems.
- **NEVER CANCEL**: NuGet restore takes 10-30 seconds depending on network.
- **Package installation**: MSIX install takes 2-5 minutes depending on system.

Always wait for operations to complete rather than assuming they've failed.

## Common Task Examples
The following are outputs from frequently run commands. Reference them instead of running bash commands to save time.

### Repository Structure
```
ls -la src/Snap.Hutao/
total 56
drwxr-xr-x  5 runner docker  4096 Aug 24 10:10 .
drwxr-xr-x  3 runner docker  4096 Aug 24 10:10 ..
-rw-r--r--  1 runner docker 13468 Aug 24 10:10 .editorconfig
-rw-r--r--  1 runner docker   368 Aug 24 10:10 .vsconfig
drwxr-xr-x 16 runner docker  4096 Aug 24 10:13 Snap.Hutao
drwxr-xr-x  2 runner docker  4096 Aug 24 10:10 Snap.Hutao.Benchmark
drwxr-xr-x  9 runner docker  4096 Aug 24 10:13 Snap.Hutao.Test
-rw-r--r--  1 runner docker  4726 Aug 24 10:10 Snap.Hutao.sln
-rw-r--r--  1 runner docker  1143 Aug 24 10:10 Snap.Hutao.slnx
-rw-r--r--  1 runner docker  1863 Aug 24 10:10 settings.xamlstyler
```

### Test Results (Expected)
```
Test summary: total: 52, failed: 5, succeeded: 47, skipped: 0, duration: 1.9s
```
47 out of 52 tests pass on non-Windows platforms. 5 expected failures due to:
- Registry access (Windows-only)
- Network connectivity restrictions
- File path format differences
- Platform-specific .NET behavior

### Tool Restoration
```
dotnet tool restore
Tool 'cake.tool' (version '5.0.0') was restored. Available commands: dotnet-cake
Restore was successful.
```