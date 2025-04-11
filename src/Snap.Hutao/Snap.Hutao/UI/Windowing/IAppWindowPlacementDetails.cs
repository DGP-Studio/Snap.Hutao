// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Foundation;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace Microsoft.UI.Windowing;
#pragma warning restore IDE0130

[WindowsRuntimeType("Microsoft.UI")]
[Guid("639EC5B2-AC0C-5BBF-8422-98DCA540D219")]
[WindowsRuntimeHelperType(typeof(global::ABI.Microsoft.UI.Windowing.IAppWindowPlacementDetails))]
[global::Windows.Foundation.Metadata.ContractVersion(typeof(WindowsAppSDKContract), 65543u)]
internal interface IAppWindowPlacementDetails
{
    RectInt32 ArrangeRect { get; }

    string DeviceName { get; }

    int Dpi { get; }

    PlacementInfo Flags { get; }

    RectInt32 NormalRect { get; }

    int ShowCmd { get; }

    RectInt32 WorkArea { get; }
}