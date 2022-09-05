// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Request.QueryString;

/// <summary>
/// A single query string parameter (name and value pair).
/// </summary>
public struct QueryStringParameter
{
    /// <summary>
    /// The name of the parameter. Cannot be null.
    /// </summary>
    public string Name;

    /// <summary>
    /// The value of the parameter (or null if there's no value).
    /// </summary>
    public string Value;

    /// <summary>
    /// Initializes a new query string parameter with the specified name and optional value.
    /// </summary>
    /// <param name="name">The name of the parameter. Cannot be null.</param>
    /// <param name="value">The optional value of the parameter.</param>
    internal QueryStringParameter(string name, string value)
    {
        Name = name;
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Name}={Value}";
    }
}
