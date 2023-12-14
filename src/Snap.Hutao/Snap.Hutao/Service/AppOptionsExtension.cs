// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Service.Game.PathAbstraction;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;

namespace Snap.Hutao.Service;

internal static class AppOptionsExtension
{
    public static NameValue<CultureInfo>? GetCurrentCultureForSelectionOrDefault(this AppOptions appOptions)
    {
        return appOptions.Cultures.SingleOrDefault(c => c.Value == appOptions.CurrentCulture);
    }
}