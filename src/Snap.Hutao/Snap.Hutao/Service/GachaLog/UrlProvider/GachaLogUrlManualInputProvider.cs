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
        MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        ValueResult<bool, string> result = await new GachaLogUrlDialog(mainWindow).GetInputUrlAsync().ConfigureAwait(false);

        if (result.IsOk)
        {
            if (result.Value.Contains("&auth_appid=webview_gacha"))
            {
                return result;
            }
            else
            {
                return new(false, "提供的Url无效");
            }
        }
        else
        {
            return new(false, null!);
        }
    }
}