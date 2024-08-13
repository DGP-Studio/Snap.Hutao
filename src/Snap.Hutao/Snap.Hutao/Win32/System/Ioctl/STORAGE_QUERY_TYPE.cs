// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.System.Ioctl;

internal enum STORAGE_QUERY_TYPE
{
    PropertyStandardQuery = 0,
    PropertyExistsQuery = 1,
    PropertyMaskQuery = 2,
    PropertyQueryMaxDefined = 3,
}