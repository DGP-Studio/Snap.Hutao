// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Factory.Picker;

internal static class FileSystemPickerInteractionExtension
{
    public static ValueResult<bool, ValueFile> PickFile(this IFileSystemPickerInteraction interaction, string? title, (string Name, string Type)[]? filters)
    {
        return interaction.PickFile(title, null, filters);
    }

    public static ValueResult<bool, string> PickFolder(this IFileSystemPickerInteraction interaction)
    {
        return interaction.PickFolder(null);
    }
}