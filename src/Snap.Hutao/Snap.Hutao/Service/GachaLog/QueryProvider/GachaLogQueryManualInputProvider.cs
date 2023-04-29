// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.View.Dialog;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 手动输入方法提供器
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient, typeof(IGachaLogQueryProvider))]
internal sealed class GachaLogQueryManualInputProvider : IGachaLogQueryProvider
{
    private readonly ITaskContext taskContext;

    /// <summary>
    /// 构造一个新的手动输入方法提供器
    /// </summary>
    /// <param name="taskContext">任务上下文</param>
    public GachaLogQueryManualInputProvider(ITaskContext taskContext)
    {
        this.taskContext = taskContext;
    }

    /// <inheritdoc/>
    public string Name { get => nameof(GachaLogQueryManualInputProvider); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();
        (bool isOk, string query) = await new GachaLogUrlDialog().GetInputUrlAsync().ConfigureAwait(false);

        if (isOk)
        {
            if (query.Contains("&auth_appid=webview_gacha"))
            {
                return new(true, new(query));
            }
            else
            {
                return new(false, SH.ServiceGachaLogUrlProviderManualInputInvalid);
            }
        }
        else
        {
            return new(false, string.Empty);
        }
    }
}