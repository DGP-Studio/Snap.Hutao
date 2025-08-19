// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using System.Globalization;

namespace Snap.Hutao.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class ElementTypeIconConverter : ValueConverter<ElementType, Uri?>
{
    public override Uri? Convert(ElementType from)
    {
        string? name = from.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture);
        return string.IsNullOrEmpty(name) ? default : ElementNameIconConverter.ElementNameToUri(name);
    }
}