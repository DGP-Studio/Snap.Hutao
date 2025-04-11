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
[WinRTExposedType(typeof(EnumTypeDetails<PlacementRestorationBehavior>))]
[global::Windows.Foundation.Metadata.ContractVersion(typeof(WindowsAppSDKContract), 65543u)]
internal enum PlacementRestorationBehavior : uint
{
    None = 0U,
    AllowShowMaximized = 0x1U,
    AllowShowFullScreen = 0x2U,
    AllowShowArranged = 0x4U,
    UseStartupInfoForFirstWindow = 0x8U,
    All = 0xFFFFFFFFU,
}