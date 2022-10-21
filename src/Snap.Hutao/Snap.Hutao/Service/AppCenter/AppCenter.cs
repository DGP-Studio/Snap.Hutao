// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Service.AppCenter.Model;
using Snap.Hutao.Service.AppCenter.Model.Log;
using Snap.Hutao.Web.Hoyolab;
using System.Net.Http;

namespace Snap.Hutao.Service.AppCenter;

[SuppressMessage("", "SA1600")]
[Injection(InjectAs.Singleton)]
public sealed class AppCenter : IDisposable
{
    private const string AppSecret = "de5bfc48-17fc-47ee-8e7e-dee7dc59d554";
    private const string API = "https://in.appcenter.ms/logs?api-version=1.0.0";

    private readonly TaskCompletionSource uploadTaskCompletionSource = new();
    private readonly CancellationTokenSource uploadTaskCancllationTokenSource = new();
    private readonly HttpClient httpClient;
    private readonly List<Log> queue;
    private readonly Device deviceInfo;
    private readonly JsonSerializerOptions options;

    private Guid sessionID;

    public AppCenter()
    {
        options = new(CoreEnvironment.JsonOptions);
        options.Converters.Add(new LogConverter());

        httpClient = new() { DefaultRequestHeaders = { { "Install-ID", CoreEnvironment.AppCenterDeviceId }, { "App-Secret", AppSecret } } };
        queue = new List<Log>();
        deviceInfo = new Device();
        Task.Run(async () =>
        {
            while (!uploadTaskCancllationTokenSource.Token.IsCancellationRequested)
            {
                await UploadAsync().ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            }

            uploadTaskCompletionSource.TrySetResult();
        }).SafeForget();
    }

    public async Task UploadAsync()
    {
        if (queue.Count == 0)
        {
            return;
        }

        string? uploadStatus = null;
        do
        {
            queue.ForEach(log => log.Status = LogStatus.Uploading);
            LogContainer container = new(queue);

            LogUploadResult? response = await httpClient
                .TryCatchPostAsJsonAsync<LogContainer, LogUploadResult>(API, container, options)
                .ConfigureAwait(false);
            uploadStatus = response?.Status;
        }
        while (uploadStatus != "Success");

        queue.RemoveAll(log => log.Status == LogStatus.Uploading);
    }

    public void Initialize()
    {
        sessionID = Guid.NewGuid();
        queue.Add(new StartServiceLog("Analytics", "Crashes").Initialize(sessionID, deviceInfo));
        queue.Add(new StartSessionLog().Initialize(sessionID, deviceInfo).Initialize(sessionID, deviceInfo));
    }

    public void TrackCrash(Exception exception, bool isFatal = true)
    {
        queue.Add(new ManagedErrorLog(exception, isFatal).Initialize(sessionID, deviceInfo));
    }

    public void TrackError(Exception exception)
    {
        queue.Add(new HandledErrorLog(exception).Initialize(sessionID, deviceInfo));
    }

    [SuppressMessage("", "VSTHRD002")]
    public void Dispose()
    {
        uploadTaskCancllationTokenSource.Cancel();
        uploadTaskCompletionSource.Task.GetAwaiter().GetResult();
    }
}