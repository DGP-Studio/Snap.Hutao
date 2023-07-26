// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Foundation.Metadata;

namespace Snap.Hutao.Core;

internal static class UniversalApiContract
{
    public static bool IsPresent(WindowsVersion version)
    {
        return ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", (ushort)version);
    }
}