// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model.Log;

[SuppressMessage("", "SA1600")]
public class EventLog : PropertiesLog
{
    public EventLog(string name)
    {
        Name = name;
    }

    [JsonPropertyName("type")]
    public override string Type { get => "event"; }

    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")]
    public string Name { get; set; }
}