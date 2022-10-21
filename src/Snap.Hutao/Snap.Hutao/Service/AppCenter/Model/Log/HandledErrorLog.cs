// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model.Log;

[SuppressMessage("", "SA1600")]
public class HandledErrorLog : PropertiesLog
{
    public HandledErrorLog(Exception exception)
    {
        Id = Guid.NewGuid();
        Exception = LogHelper.Create(exception);
    }

    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("exception")]
    public AppCenterException Exception { get; set; }

    [JsonPropertyName("type")]
    public override string Type { get => "handledError"; }
}