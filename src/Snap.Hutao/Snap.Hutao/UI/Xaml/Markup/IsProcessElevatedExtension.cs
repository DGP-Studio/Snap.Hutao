// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;
using Snap.Hutao.Core;

namespace Snap.Hutao.UI.Xaml.Markup;

[MarkupExtensionReturnType(ReturnType = typeof(bool))]
internal sealed partial class IsProcessElevatedExtension : MarkupExtension
{
    protected override object ProvideValue()
    {
        return HutaoRuntime.IsProcessElevated;
    }
}