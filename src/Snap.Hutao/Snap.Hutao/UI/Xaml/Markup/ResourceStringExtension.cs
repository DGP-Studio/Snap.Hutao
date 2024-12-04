// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Markup;
using System.Globalization;

namespace Snap.Hutao.UI.Xaml.Markup;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
internal sealed partial class ResourceStringExtension : MarkupExtension
{
    public SHName Name { get; set; }

    public string? CultureName { get; set; }

    protected override object ProvideValue()
    {
        CultureInfo cultureInfo = CultureName is not null ? CultureInfo.GetCultureInfo(CultureName) : CultureInfo.CurrentCulture;
        return SH.ResourceManager.GetString(string.Intern(Name.ToString()), cultureInfo) ?? string.Empty;
    }
}