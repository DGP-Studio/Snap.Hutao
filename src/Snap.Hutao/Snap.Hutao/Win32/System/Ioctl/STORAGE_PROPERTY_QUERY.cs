// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Ioctl;

internal struct STORAGE_PROPERTY_QUERY
{
    public STORAGE_PROPERTY_ID PropertyId;
    public STORAGE_QUERY_TYPE QueryType;
    public FlexibleArray<byte> AdditionalParameters;
}