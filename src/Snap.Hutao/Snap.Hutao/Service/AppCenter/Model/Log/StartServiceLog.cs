// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model.Log;

[SuppressMessage("", "SA1600")]
public class StartServiceLog : Log
{
    public StartServiceLog(params string[] services)
    {
        Services = services;
    }

    [JsonPropertyName("services")]
    public string[] Services { get; set; }

    [JsonPropertyName("type")]
    public override string Type { get => "startService"; }
}