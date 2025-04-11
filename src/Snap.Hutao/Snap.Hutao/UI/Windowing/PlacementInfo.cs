// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Foundation;
using WinRT;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace Microsoft.UI.Windowing;
#pragma warning restore IDE0130

[Flags]
[WindowsRuntimeType("Microsoft.UI")]
[WinRTExposedType(typeof(EnumTypeDetails<PlacementInfo>))]
[global::Windows.Foundation.Metadata.ContractVersion(typeof(WindowsAppSDKContract), 65543u)]
internal enum PlacementInfo : uint
{
    None = 0U,
    RestoreToMaximized = 0x2U,
    RestoreToArranged = 0x8U,
    Arranged = 0x10U,
    Resizable = 0x20U,
    FullScreen = 0x40U,
}