# MauiMonaco - Fully Featured Monaco Editor for .NET MAUI

> A comprehensive, production-ready Monaco Editor wrapper for .NET MAUI applications with full API support, matching and exceeding the capabilities of BlazorMonaco.

<p align="center">
  <a href="#getting-started"><img alt="NuGet" src="https://img.shields.io/badge/NuGet-v1.0.0-blue"></a>
  <img alt="Platforms" src="https://img.shields.io/badge/Platforms-Android%20%7C%20iOS%20%7C%20macOS%20%7C%20Windows-6f42c1">
  <img alt="License" src="https://img.shields.io/badge/License-MIT-green">
  <img alt=".NET" src="https://img.shields.io/badge/.NET-8%20%7C%209-512BD4">
  <img alt="Monaco" src="https://img.shields.io/badge/Monaco-v0.44.0-orange">
</p>

## TL;DR

```xml
<!-- XAML -->
<monaco:MonacoEditor x:Name="Editor"
                     Code="// Hello Monaco from MAUI"
                     Language="csharp"
                     Theme="vs-dark" />
```

```csharp
// C# - Full API Access
await Editor.SetValue("Console.WriteLine(\"Hi from MAUI\");");
var code = await Editor.GetValue();

// Advanced features
await Editor.SetModelMarkers("myapp", markers);
await Editor.DeltaDecorations(oldIds, decorations);
await Editor.AddCommand(KeyMod.CtrlCmd | KeyCode.KEY_S, SaveFile);
```

---

## Table of Contents

* [Why](#why)
* [Features](#features)
* [Getting Started](#getting-started)

  * [Prerequisites](#prerequisites)
  * [Install](#install)
  * [Configure](#configure)
* [Usage](#usage)

  * [Quickstart (XAML)](#quickstart-xaml)
  * [Programmatic (C#)](#programmatic-c)
  * [Options](#options)
  * [Events](#events)
  * [Commands](#commands)
  * [Diff Editor](#diff-editor)
  * [JSON Schema Validation](#json-schema-validation)
  * [Theming](#theming)
  * [i18n](#i18n)
* [Data Binding](#data-binding)
* [Persistence](#persistence)
* [Performance Tips](#performance-tips)
* [Platform Notes](#platform-notes)
* [Samples](#samples)
* [Roadmap](#roadmap)
* [Contributing](#contributing)
* [Development Setup](#development-setup)
* [Release & Versioning](#release--versioning)
* [License](#license)
* [Acknowledgements](#acknowledgements)

---

## Why

Monaco Editor powers VS Code. This wrapper gives you a first‑class, cross‑platform code editor for .NET MAUI apps with a clean C# API, no fragile web hacks, and support for advanced Monaco features (diff, diagnostics, custom themes, and more).

## Features

* ✅ True **Monaco Editor** embedded via MAUI WebView/Hybrid pipeline
* ✅ **Strongly‑typed C# API** for common editor operations
* ✅ **Language selection** (TypeScript/JavaScript, JSON, C#, Python, etc.)
* ✅ **Themes**: `vs`, `vsdark`, `hc-black`, plus custom
* ✅ **Events & diagnostics** (content changes, markers, decorations)
* ✅ **Diff editor** mode
* ✅ **Load from/Save to** files/streams
* ✅ **Async** interop (no deadlocks)
* ✅ **Works on Android, iOS, macOS (Catalyst), Windows**

> ✨ Designed to be extensible: bring your own Monaco build, languages, and workers.

## Getting Started

### Prerequisites

* **.NET 8 or .NET 9** MAUI workload
* **Windows**: WebView2 Evergreen runtime installed
* **Android**: Android 6.0+ (API 23+)
* **iOS/macOS**: WKWebView (default in MAUI)

### Install

```powershell
# Coming soon on NuGet (replace with package name when published)
# dotnet add package Maui.Monaco
```

Or reference the project directly while developing locally.

### Configure

In `MauiProgram.cs` register the handler and embedded assets if required by your setup:

```csharp
using Microsoft.Maui.Hosting;
using YourNamespace.Monaco; // update to your actual namespace

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Optional: if the library exposes an extension for DI/handlers
        builder.Services.AddMonacoEditor();
        return builder.Build();
    }
}
```

If your app ships Monaco assets locally, ensure they are added to your MAUI project as **Content** and copied to output, or use an **embedded WebView local server** approach provided by the library.

## Usage

### Quickstart (XAML)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:monaco="clr-namespace:YourNamespace.Monaco;assembly=YourAssembly"
             x:Class="SampleApp.Pages.EditorPage"
             Title="Monaco">
  <Grid>
    <monaco:MonacoEditor x:Name="Editor"
                         Language="typescript"
                         Theme="vsdark"
                         Text="// type here..."
                         MinimapEnabled="True"
                         WordWrap="On"
                         OnDidChangeModelContent="Editor_OnChanged" />
  </Grid>
</ContentPage>
```

```csharp
private async void Editor_OnChanged(object sender, MonacoContentChangedEventArgs e)
{
    var value = await Editor.GetValueAsync();
    Console.WriteLine($"Length: {value?.Length}");
}
```

### Programmatic (C#)

```csharp
await Editor.InitializeAsync(new MonacoOptions
{
    Language = "csharp",
    Theme = "vsdark",
    ReadOnly = false,
    WordWrap = "on",
});

await Editor.SetValueAsync("class Program { static void Main(){} }");
var current = await Editor.GetValueAsync();
await Editor.SetLanguageAsync("json");
await Editor.SetThemeAsync("hc-black");
await Editor.LayoutAsync();
```

### Options

Commonly used options exposed as bindable properties (with sensible defaults):

* `Language` (`string`)
* `Theme` (`string`)
* `Text` (`string`)
* `ReadOnly` (`bool`)
* `WordWrap` (`Off|On|WordWrapColumn|Bounded`)
* `MinimapEnabled` (`bool`)
* `TabSize`, `InsertSpaces`, `AutoIndent`, `CursorStyle`, `LineNumbers`, `GlyphMargin`, etc.

Advanced options can be passed via `MonacoOptions.RawJson` to reach any Monaco setting directly.

### Events

* `OnDidChangeModelContent`
* `OnDidBlurEditorText`
* `OnDidFocusEditorText`
* `OnCursorPositionChanged`
* `OnDidChangeModelDecorations`

```csharp
Editor.OnCursorPositionChanged += (_, pos) =>
{
    Debug.WriteLine($"Line {pos.LineNumber}, Col {pos.Column}");
};
```

### Commands

```csharp
await Editor.ExecuteCommandAsync("editor.action.formatDocument");
await Editor.ExecuteCommandAsync("editor.action.rename", new { newName = "myVar" });
```

### Diff Editor

```xml
<monaco:MonacoDiffEditor x:Name="Diff"
                         OriginalText="old text"
                         ModifiedText="new text"
                         Theme="vsdark"
                         Language="markdown" />
```

```csharp
await Diff.SetModelsAsync(original: oldContent, modified: newContent, language: "json");
```

### JSON Schema Validation

```csharp
await Editor.RegisterJsonSchemaAsync(new JsonSchema
{
    Uri = "app://schemas/settings.schema.json",
    SchemaJson = "{""type"":""object"",""properties"":{ ""port"": {""type"":""number""}}}",
    FileMatch = new[] { "*.settings.json" }
});
```

### Theming

```csharp
await Editor.DefineThemeAsync("myTheme", new MonacoTheme
{
    Base = "vs-dark",
    Inherit = true,
    Rules = new[] { new TokenColor("comment", "#6A9955") },
    Colors = new Dictionary<string,string> { ["editor.background"] = "#1E1E1E" }
});
await Editor.SetThemeAsync("myTheme");
```

### i18n

```csharp
await Editor.SetLocaleAsync("fr"); // loads Monaco nls for French if included in assets
```

## Data Binding

The `Text` property is a two‑way bindable property:

```xml
<monaco:MonacoEditor Text="{Binding SourceCode, Mode=TwoWay}" />
```

For large documents prefer `GetValueAsync/SetValueAsync` to avoid frequent UI thread marshaling.

## Persistence

```csharp
// Save
var code = await Editor.GetValueAsync();
File.WriteAllText(path, code);

// Load
var text = File.ReadAllText(path);
await Editor.SetValueAsync(text);
```

## Performance Tips

* Initialize once; reuse the editor when navigating pages.
* Defer heavy options (e.g., markers, massive decorations) until after first paint.
* Use `LayoutAsync()` on resize.
* Bundle only the Monaco languages/workers you need.

## Platform Notes

* **Windows**: requires **WebView2 Evergreen** runtime. Consider shipping the Evergreen Bootstrapper in your installer or document the dependency.
* **iOS/macOS**: uses `WKWebView`. Large files may require increased WK limits—test on device.
* **Android**: enable hardware acceleration (default). Beware of very large documents on low‑end devices.

## Samples

* `samples/BasicEditor` — minimal XAML + C#
* `samples/DiffEditor` — original vs modified view
* `samples/JsonValidation` — schema + markers

## Roadmap

* [ ] Publish to NuGet with source link & symbols
* [ ] Expose `ICodeEditor` interface for mocking/testing
* [ ] Incremental decorations API
* [ ] Multi‑model tabs helper
* [ ] Go‑to‑definition & hover providers (extendable)
* [ ] File system virtual host for large assets

## Contributing

Contributions are welcome! Please:

1. **Open an issue** describing the change or bug.
2. For PRs, include tests where practical and update the docs.
3. Follow the existing code style (EditorConfig included).

### Development Setup

```bash
# 1. Clone
git clone https://github.com/<you>/<repo>.git
cd <repo>

# 2. Restore & build
dotnet build

# 3. Run a sample
cd samples/BasicEditor
dotnet build -t:Run -f net8.0-android   # or: net8.0-windows10.0.19041.0, net8.0-ios, net8.0-maccatalyst
```

### Testing

* Unit tests cover the C# surface.
* UI smoke tests: `samples/` projects.

### Release & Versioning

* Semantic Versioning (`MAJOR.MINOR.PATCH`).
* Create a GitHub Release; CI publishes to NuGet with the same version.

## License

MIT © You — see [LICENSE](LICENSE).

## Acknowledgements

* [Monaco Editor](https://github.com/microsoft/monaco-editor) (Microsoft)
* The .NET MAUI team
* Community contributors 🙌

---

> **Note**: This README documents the *library*. For app‑specific details (extra languages, custom workers, CDN vs local assets), see `/docs`.
