// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model.Log;

[SuppressMessage("", "SA1600")]
public class LogContainer
{
    public LogContainer(IEnumerable<Log> logs)
    {
        Logs = logs;
    }

    [JsonPropertyName("logs")]
    public IEnumerable<Log> Logs { get; set; }
}