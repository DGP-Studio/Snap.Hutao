// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace ABI.Microsoft.UI.Content;
#pragma warning restore IDE0130

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class ContentExternalBackdropLinkRcwFactoryAttribute : global::WinRT.WinRTImplementationTypeRcwFactoryAttribute
{
    public override object CreateInstance(global::WinRT.IInspectable inspectable)
    {
        return new global::Microsoft.UI.Content.ContentExternalBackdropLink(inspectable.ObjRef);
    }
}