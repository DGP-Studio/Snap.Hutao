// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.View.Dialog;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 手动输入方法
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient, typeof(IGachaLogQueryProvider))]
internal sealed class GachaLogQueryManualInputProvider : IGachaLogQueryProvider
{
    /// <inheritdoc/>
    public string Name { get => nameof(GachaLogQueryManualInputProvider); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        // ContentDialog must be created by main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        (bool isOk, string query) = await new GachaLogUrlDialog().GetInputUrlAsync().ConfigureAwait(false);

        if (isOk)
        {
            if (query.Contains("&auth_appid=webview_gacha"))
            {
                return new(true, new(query, query.Contains("hoyoverse.com")));
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