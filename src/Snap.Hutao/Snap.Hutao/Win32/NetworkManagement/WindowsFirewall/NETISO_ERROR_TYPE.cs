// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.NetworkManagement.WindowsFirewall;

internal enum NETISO_ERROR_TYPE
{
    NETISO_ERROR_TYPE_NONE = 0,
    NETISO_ERROR_TYPE_PRIVATE_NETWORK = 1,
    NETISO_ERROR_TYPE_INTERNET_CLIENT = 2,
    NETISO_ERROR_TYPE_INTERNET_CLIENT_SERVER = 3,
    NETISO_ERROR_TYPE_MAX = 4,
}