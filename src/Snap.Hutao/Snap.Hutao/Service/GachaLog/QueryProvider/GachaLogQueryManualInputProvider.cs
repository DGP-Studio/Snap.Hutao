// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.UI.Xaml.View.Dialog;
using System.Collections.Specialized;
using System.Web;

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
    private readonly CultureOptions cultureOptions;

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        GachaLogUrlDialog dialog = await contentDialogFactory.CreateInstanceAsync<GachaLogUrlDialog>().ConfigureAwait(false);
        (bool isOk, string queryString) = await dialog.GetInputUrlAsync().ConfigureAwait(false);

        if (isOk)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(queryString);

            if (query.TryGetSingleValue("auth_appid", out string? appId) && appId is "webview_gacha")
            {
                string? queryLanguageCode = query["lang"];
                if (cultureOptions.LanguageCodeFitsCurrentLocale(queryLanguageCode))
                {
                    return new(true, new(queryString));
                }
                else
                {
                    string message = SH.FormatServiceGachaLogUrlProviderUrlLanguageNotMatchCurrentLocale(queryLanguageCode, cultureOptions.LanguageCode);
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