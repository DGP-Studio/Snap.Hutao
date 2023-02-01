// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.View.Dialog;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 手动输入方法
/// </summary>
[Injection(InjectAs.Transient, typeof(IGachaLogUrlProvider))]
internal class GachaLogUrlManualInputProvider : IGachaLogUrlProvider
{
    /// <inheritdoc/>
    public string Name { get => nameof(GachaLogUrlManualInputProvider); }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> GetQueryAsync()
    {
        // ContentDialog must be created by main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        ValueResult<bool, string> result = await new GachaLogUrlDialog().GetInputUrlAsync().ConfigureAwait(false);

        if (result.IsOk)
        {
            if (result.Value.Contains("&auth_appid=webview_gacha"))
            {
                return result;
            }
            else
            {
                return new(false, SH.ServiceGachaLogUrlProviderManualInputInvalid);
            }
        }
        else
        {
            return new(false, null!);
        }
    }
}