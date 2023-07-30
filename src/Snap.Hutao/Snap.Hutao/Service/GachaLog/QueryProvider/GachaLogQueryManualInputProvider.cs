// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Metadata;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Request.QueryString;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 手动输入方法提供器
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Transient)]
internal sealed partial class GachaLogQueryManualInputProvider : IGachaLogQueryProvider
{
    private readonly IServiceProvider serviceProvider;
    private readonly MetadataOptions metadataOptions;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        // ContentDialog must be created by main thread.
        await taskContext.SwitchToMainThreadAsync();
        GachaLogUrlDialog dialog = serviceProvider.CreateInstance<GachaLogUrlDialog>();
        (bool isOk, string queryString) = await dialog.GetInputUrlAsync().ConfigureAwait(false);

        if (isOk)
        {
            QueryString query = QueryString.Parse(queryString);
            string queryLanguageCode = query["lang"];
            if (query["auth_appid"] == "webview_gacha")
            {
                if (metadataOptions.IsCurrentLocale(queryLanguageCode))
                {
                    return new(true, new(queryString));
                }
                else
                {
                    string message = string.Format(
                        SH.ServiceGachaLogUrlProviderUrlLanguageNotMatchCurrentLocale,
                        queryLanguageCode,
                        metadataOptions.LanguageCode);
                    return new(false, message);
                }
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