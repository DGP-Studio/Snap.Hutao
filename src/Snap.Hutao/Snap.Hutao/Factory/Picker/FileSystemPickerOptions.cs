// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Factory.Picker;

internal sealed class FileSystemPickerOptions
{
    public string? Title { get; init; }

    public string? DefaultFileName { get; init; }

    public string? FilterName { get; init; }

    public string? FilterType { get; init; }
}