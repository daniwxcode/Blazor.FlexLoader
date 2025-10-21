# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2024-10-21

### Added
- ?? **Bilingual documentation** (French/English)
- ?? **Comprehensive XML documentation** with code examples
- ?? **Detailed usage guide** in `docs/LoaderService-Documentation.md`
- ?? **Best practices** and advanced usage examples
- ?? **Bilingual README** with navigation
- ??? **Package tags** for better discoverability

### Changed
- ?? **Enhanced package description** in both languages
- ?? **Updated author** to `daniwxcode`
- ?? **Documentation files** now included in NuGet package

### Technical
- ? **XML documentation** for all public methods
- ?? **Cross-references** between related methods
- ?? **Warning documentation** for critical methods
- ?? **IntelliSense support** improved

## [1.1.0] - 2024-10-21

### Added
- ? **New `Show()` method** - Simple alias for `Increment()`
- ? **New `Close()` method** - Simple alias for `Decrement()`
- ?? **XML documentation** for all methods
- ?? **Backward compatibility** maintained

### Changed
- ?? **Updated README** with new usage examples
- ?? **Simplified API** for common use cases

## [1.0.0] - 2024-10-21

### Added
- ?? **Initial release** of Blazor.FlexLoader
- ?? **LoaderService** with thread-safe counter
- ?? **FlexLoader component** with customizable UI
- ??? **Support for custom images** (GIF, SVG, PNG, etc.)
- ?? **Custom text** support
- ?? **Custom content** with RenderFragment
- ?? **Flexible positioning** (absolute/fixed)
- ?? **Customizable colors** and styles
- ? **Auto-close** functionality
- ?? **Click-to-close** overlay option
- ?? **Service collection extensions** for easy setup
- ?? **NuGet package** configuration
- ?? **MIT License**
- ?? **GitHub repository** setup

### Technical
- ?? **Targets .NET 9**
- ?? **Thread-safe operations** with `Interlocked`
- ?? **Event-driven** state management
- ?? **Comprehensive examples** and documentation

[1.2.0]: https://github.com/daniwxcode/Blazor.FlexLoader/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/daniwxcode/Blazor.FlexLoader/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/daniwxcode/Blazor.FlexLoader/releases/tag/v1.0.0