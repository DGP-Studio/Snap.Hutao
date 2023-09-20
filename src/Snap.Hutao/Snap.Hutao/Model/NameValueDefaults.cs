// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

internal sealed class NameValueDefaults
{
    private static readonly NameValue<string> StringValue = new(SH.ModelNameValueDefaultName, SH.ModelNameValueDefaultDescription);

    public static NameValue<string> String
    {
        get => StringValue;
    }
}