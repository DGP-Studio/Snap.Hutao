// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Factory.Picker;

internal interface IFileSystemPickerInteraction
{
    ValueResult<bool, ValueFile> PickFile(string? title, string? defaultFileName, (string Name, string Type)[]? filters);

    ValueResult<bool, string> PickFolder(string? title);

    ValueResult<bool, ValueFile> SaveFile(string? title, string? defaultFileName, (string Name, string Type)[]? filters);
}