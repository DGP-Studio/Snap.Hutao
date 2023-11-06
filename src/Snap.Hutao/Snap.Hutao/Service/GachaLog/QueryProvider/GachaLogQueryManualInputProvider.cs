// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
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
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly MetadataOptions metadataOptions;

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        GachaLogUrlDialog dialog = await contentDialogFactory.CreateInstanceAsync<GachaLogUrlDialog>().ConfigureAwait(false);
        (bool isOk, string queryString) = await dialog.GetInputUrlAsync().ConfigureAwait(false);

        if (isOk)
        {
            QueryString query = QueryString.Parse(queryString);
            if (query.TryGetValue("auth_appid", out string? appId) && appId is "webview_gacha")
            {
                string queryLanguageCode = query["lang"];
                if (metadataOptions.IsCurrentLocale(queryLanguageCode))
                {
                    return new(true, new(queryString));
                }
                else
                {
                    string message = SH.ServiceGachaLogUrlProviderUrlLanguageNotMatchCurrentLocale
                        .Format(queryLanguageCode, metadataOptions.LanguageCode);
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