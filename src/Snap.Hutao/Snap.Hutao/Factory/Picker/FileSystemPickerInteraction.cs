// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.LifeCycle;

namespace Snap.Hutao.Factory.Picker;

[GeneratedConstructor]
[Service(ServiceLifetime.Transient, typeof(IFileSystemPickerInteraction))]
internal sealed partial class FileSystemPickerInteraction : IFileSystemPickerInteraction
{
    private readonly ICurrentXamlWindowReference currentWindowReference;

    public ValueResult<bool, ValueFile> PickFile(string? title, string? defaultFileName, string? filterName, string? filterType)
    {
        bool picked = FileSystem.PickFile(currentWindowReference.GetWindowHandle(), title, defaultFileName, filterName, filterType, out string? path);
        return new(picked, path);
    }

    public ValueResult<bool, ValueFile> SaveFile(string? title, string? defaultFileName, string? filterName, string? filterType)
    {
        bool picked = FileSystem.SaveFile(currentWindowReference.GetWindowHandle(), title, defaultFileName, filterName, filterType, out string? path);
        return new(picked, path);
    }

    public ValueResult<bool, string?> PickFolder(string? title)
    {
        bool picked = FileSystem.PickFolder(currentWindowReference.GetWindowHandle(), title, out string? path);
        return new(picked, path);
    }
}