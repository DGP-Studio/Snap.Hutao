// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace Snap.Hutao.Web.Hoyolab;

internal static partial class HoyolabRegex
{
    [GeneratedRegex("^(?:1|)[1-9][0-9]{8}$")]
    public static partial Regex UidRegex { get; }

    [GeneratedRegex("^(cn_gf01|cn_qd01|os_usa|os_euro|os_asia|os_cht)$")]
    public static partial Regex RegionRegex { get; }
}
