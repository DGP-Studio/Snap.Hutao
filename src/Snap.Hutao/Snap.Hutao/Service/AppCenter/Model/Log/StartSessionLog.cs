// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model.Log;

[SuppressMessage("", "SA1600")]
public class StartSessionLog : Log
{
    [JsonPropertyName("type")]
    public override string Type { get => "startSession"; }
}