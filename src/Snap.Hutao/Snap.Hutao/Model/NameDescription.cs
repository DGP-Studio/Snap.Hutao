// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

internal class NameDescription
{
    public NameDescription()
    {
    }

    public NameDescription(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static NameDescription Default { get; } = new(SH.ModelNameValueDefaultName, SH.ModelNameValueDefaultDescription);

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;
}