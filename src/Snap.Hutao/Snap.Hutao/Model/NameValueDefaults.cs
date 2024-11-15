// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

internal sealed class NameValueDefaults
{
    public static NameValue<string> String { get; } = new(SH.ModelNameValueDefaultName, SH.ModelNameValueDefaultDescription);
}