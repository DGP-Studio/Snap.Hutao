// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// DS2请求
/// </summary>
public class DynamicSecrect2Playload
{
    /// <summary>
    /// q
    /// </summary>
    [JsonPropertyName("query")]
    public Dictionary<string, string> Query { get; set; } = default!;

    /// <summary>
    /// b
    /// </summary>
    [JsonPropertyName("body")]
    public string Body { get; set; } = default!;
}
