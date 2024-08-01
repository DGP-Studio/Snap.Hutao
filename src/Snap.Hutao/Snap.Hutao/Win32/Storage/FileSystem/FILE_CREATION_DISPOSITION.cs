// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Storage.FileSystem;

internal enum FILE_CREATION_DISPOSITION : uint
{
    CREATE_NEW = 1U,
    CREATE_ALWAYS = 2U,
    OPEN_EXISTING = 3U,
    OPEN_ALWAYS = 4U,
    TRUNCATE_EXISTING = 5U,
}