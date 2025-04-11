// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using WinRT;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace ABI.Microsoft.UI.Windowing;
#pragma warning restore IDE0130

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class AppWindowPlacementDetailsRcwFactoryAttribute : WinRTImplementationTypeRcwFactoryAttribute
{
    public override object CreateInstance(IInspectable inspectable)
    {
        return new global::Microsoft.UI.Windowing.AppWindowPlacementDetails(inspectable.ObjRef);
    }
}