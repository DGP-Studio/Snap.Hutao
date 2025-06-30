// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Factory.Picker;

internal static class FileSystemPickerInteractionExtension
{
    public static ValueResult<bool, ValueFile> PickFile(this IFileSystemPickerInteraction interaction, FileSystemPickerOptions options)
    {
        return interaction.PickFile(options.Title, options.DefaultFileName, options.FilterName, options.FilterType);
    }

    public static ValueResult<bool, ValueFile> SaveFile(this IFileSystemPickerInteraction interaction, FileSystemPickerOptions options)
    {
        return interaction.SaveFile(options.Title, options.DefaultFileName, options.FilterName, options.FilterType);
    }

    public static ValueResult<bool, ValueFile> PickFile(this IFileSystemPickerInteraction interaction, string? title, string? filterName, string? filterType)
    {
        return interaction.PickFile(title, null, filterName, filterType);
    }

    public static ValueResult<bool, string?> PickFolder(this IFileSystemPickerInteraction interaction)
    {
        return interaction.PickFolder(null);
    }
}