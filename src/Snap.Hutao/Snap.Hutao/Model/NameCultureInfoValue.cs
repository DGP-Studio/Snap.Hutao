// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Model;

internal sealed class NameCultureInfoValue : NameValue<CultureInfo>
{
    public NameCultureInfoValue(string name, CultureInfo value)
        : base(name, value)
    {
    }
}