// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Storage.FileSystem;

[Flags]
internal enum FILE_SHARE_MODE : uint
{
    FILE_SHARE_NONE = 0U,
    FILE_SHARE_DELETE = 4U,
    FILE_SHARE_READ = 1U,
    FILE_SHARE_WRITE = 2U,
}