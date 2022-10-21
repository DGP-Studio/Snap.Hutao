// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using System.Diagnostics;

namespace Snap.Hutao.Service.AppCenter.Model.Log;

[SuppressMessage("", "SA1600")]
public class ManagedErrorLog : Log
{
    public ManagedErrorLog(Exception exception, bool fatal = true)
    {
        var p = Process.GetCurrentProcess();
        Id = Guid.NewGuid();
        Fatal = fatal;
        UserId = CoreEnvironment.AppCenterDeviceId;
        ProcessId = p.Id;
        Exception = LogHelper.Create(exception);
        ProcessName = p.ProcessName;
        Architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
        AppLaunchTimestamp = p.StartTime.ToUniversalTime();
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("processId")]
    public int ProcessId { get; set; }

    [JsonPropertyName("processName")]
    public string ProcessName { get; set; }

    [JsonPropertyName("fatal")]
    public bool Fatal { get; set; }

    [JsonPropertyName("appLaunchTimestamp")]
    public DateTime? AppLaunchTimestamp { get; set; }

    [JsonPropertyName("architecture")]
    public string? Architecture { get; set; }

    [JsonPropertyName("exception")]
    public AppCenterException Exception { get; set; }

    [JsonPropertyName("type")]
    public override string Type { get => "managedError"; }
}