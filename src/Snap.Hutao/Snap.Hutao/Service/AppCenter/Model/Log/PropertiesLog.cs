// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model.Log;

[SuppressMessage("", "SA1600")]
public abstract class PropertiesLog : Log
{
    [JsonPropertyName("properties")]
    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
}