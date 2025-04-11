// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Foundation;
using System.Runtime.InteropServices;
using WinRT;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace Microsoft.UI.Windowing;
#pragma warning restore IDE0130

[WindowsRuntimeType("Microsoft.UI")]
[Guid("04DB96C7-DEB6-5BE4-BFDC-1BC0361C8A12")]
[WindowsRuntimeHelperType(typeof(global::ABI.Microsoft.UI.Windowing.IAppWindowExperimental))]
[global::Windows.Foundation.Metadata.ContractVersion(typeof(WindowsAppSDKContract), 65543u)]
internal interface IAppWindowExperimental
{
    AppWindowPlacementDetails GetCurrentPlacement();

    void SaveCurrentPlacement();

    bool SetCurrentPlacement(AppWindowPlacementDetails placementDetails, bool isFirstWindow);

#pragma warning disable SA1201
    Guid? PersistedStateId { get; set; }
#pragma warning restore SA1201

    PlacementRestorationBehavior PlacementRestorationBehavior { get; set; }
}