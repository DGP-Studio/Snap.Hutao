// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata;

internal sealed class ParameterDescription
{
    public ParameterDescription(string parameter, string description)
    {
        Parameter = parameter;
        Description = description;
    }

    public string Parameter { get; }

    public string Description { get; }
}