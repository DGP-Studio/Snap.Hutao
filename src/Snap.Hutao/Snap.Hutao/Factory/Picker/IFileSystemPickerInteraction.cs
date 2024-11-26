// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Snap.Hutao.Core.IO;

namespace Snap.Hutao.Factory.Picker;

internal interface IFileSystemPickerInteraction
{
    ValueResult<bool, ValueFile> PickFile([LocalizationRequired] string? title, string? defaultFileName, (string Name, string Type)[]? filters);

    ValueResult<bool, string> PickFolder([LocalizationRequired] string? title);

    ValueResult<bool, ValueFile> SaveFile([LocalizationRequired] string? title, string? defaultFileName, (string Name, string Type)[]? filters);
}