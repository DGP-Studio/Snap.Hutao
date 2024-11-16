// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using System.Collections.Specialized;
using System.Web;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGachaLogQueryProvider), Key = RefreshOption.SToken)]
internal sealed partial class GachaLogQuerySTokenProvider : IGachaLogQueryProvider
{
    private readonly IInfoBarService infoBarService;
    private readonly BindingClient2 bindingClient2;
    private readonly CultureOptions cultureOptions;
    private readonly IUserService userService;

    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            return new(false, GachaLogQuery.Invalid(SH.MustSelectUserAndUid));
        }

        if (userAndUid.User.IsOversea)
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderStokenUnsupported));
        }

        GenAuthKeyData data = GenAuthKeyData.CreateForWebViewGacha(userAndUid.Uid);
        Response<GameAuthKey> authkeyResponse = await bindingClient2.GenerateAuthenticationKeyAsync(userAndUid.User, data).ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(authkeyResponse, infoBarService, out GameAuthKey? authKey))
        {
            return new(false, GachaLogQuery.Invalid(SH.ServiceGachaLogUrlProviderAuthkeyRequestFailed));
        }

        return new(true, new(ComposeQueryString(data, authKey, cultureOptions.LanguageCode)));
    }

    private static string ComposeQueryString(GenAuthKeyData genAuthKeyData, GameAuthKey gameAuthKey, string lang)
    {
        NameValueCollection collection = HttpUtility.ParseQueryString(string.Empty);
        collection.Set("lang", lang);
        collection.Set("auth_appid", genAuthKeyData.AuthAppId);
        collection.Set("authkey", gameAuthKey.AuthKey);
        collection.Set("authkey_ver", $"{gameAuthKey.AuthKeyVersion:D}");
        collection.Set("sign_type", $"{gameAuthKey.SignType:D}");

        string? result = collection.ToString();
        ArgumentNullException.ThrowIfNull(result);
        return result;
    }
}