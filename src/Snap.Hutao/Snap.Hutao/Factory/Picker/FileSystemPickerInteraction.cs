// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Factory.Picker;

[ConstructorGenerated]
[Service(ServiceLifetime.Transient, typeof(IFileSystemPickerInteraction))]
internal sealed partial class FileSystemPickerInteraction : IFileSystemPickerInteraction
{
    private readonly ICurrentXamlWindowReference<MainWindow> mainWindowReference;
    private readonly ICurrentXamlWindowReference<GuideWindow> guideWindowReference;

    public ValueResult<bool, ValueFile> PickFile(string? title, string? defaultFileName, string? filterName, string? filterType)
    {
        bool picked = FileSystem.PickFile(GetWindowHandle(), title, defaultFileName, filterName, filterType, out string? path);
        return new(picked, path);
    }

    public ValueResult<bool, ValueFile> SaveFile(string? title, string? defaultFileName, string? filterName, string? filterType)
    {
        bool picked = FileSystem.SaveFile(GetWindowHandle(), title, defaultFileName, filterName, filterType, out string? path);
        return new(picked, path);
    }

    public ValueResult<bool, string?> PickFolder(string? title)
    {
        bool picked = FileSystem.PickFolder(GetWindowHandle(), title, out string? path);
        return new(picked, path);
    }

    private HWND GetWindowHandle()
    {
        return mainWindowReference.Window?.GetWindowHandle() ?? guideWindowReference.Window?.GetWindowHandle() ?? default;
    }
}