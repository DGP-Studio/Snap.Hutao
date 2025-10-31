// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.UI.Xaml.View.Dialog;
using System.Collections.Specialized;
using System.Web;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

[Service(ServiceLifetime.Transient, typeof(IGachaLogQueryProvider), Key = RefreshOptionKind.ManualInput)]
internal sealed partial class GachaLogQueryManualInputProvider : IGachaLogQueryProvider
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly CultureOptions cultureOptions;

    [GeneratedConstructor]
    public partial GachaLogQueryManualInputProvider(IServiceProvider serviceProvider);

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        GachaLogUrlDialog dialog = await contentDialogFactory.CreateInstanceAsync<GachaLogUrlDialog>(serviceProvider).ConfigureAwait(false);
        if (await dialog.GetInputUrlAsync().ConfigureAwait(false) is not (true, { } url))
        {
            return new(false, default);
        }

        if ((AfterLast(url, "index.html") ?? AfterLast(url, "getGachaLog")) is not { } queryString)
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderManualInputInvalid));
        }

        NameValueCollection query = HttpUtility.ParseQueryString(queryString);
        if (!query.TryGetSingleValue("auth_appid", out string? appId) || appId is not "webview_gacha")
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderManualInputInvalid));
        }

        if (!query.TryGetSingleValue("lang", out string? queryLanguageCode) || !LocaleNames.LanguageCodeFitsCurrentLocale(queryLanguageCode, cultureOptions.LocaleName))
        {
            string message = SH.FormatServiceGachaLogUrlProviderUrlLanguageNotMatchCurrentLocale(queryLanguageCode, cultureOptions.LanguageCode);
            return new(false, GachaLogQuery.Invalid(message));
        }

        return new(true, new(url));
    }

    private static string? AfterLast(string url, string match)
    {
        ReadOnlySpan<char> urlSpan = url;

        int index = urlSpan.LastIndexOf(match);
        if (index >= 0)
        {
            index += match.Length;
            return urlSpan[index..].ToString();
        }

        return default;
    }
}