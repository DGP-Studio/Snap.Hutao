// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Request;
using Snap.Hutao.Web.Response;
using System.Collections.Specialized;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 使用 SToken 提供祈愿 Url
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Transient)]
internal sealed partial class GachaLogQuerySTokenProvider : IGachaLogQueryProvider
{
    private readonly BindingClient2 bindingClient2;
    private readonly CultureOptions cultureOptions;
    private readonly IUserService userService;

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            if (userAndUid.User.IsOversea)
            {
                return new(false, SH.ServiceGachaLogUrlProviderStokenUnsupported);
            }

            GenAuthKeyData data = GenAuthKeyData.CreateForWebViewGacha(userAndUid.Uid);
            Response<GameAuthKey> authkeyResponse = await bindingClient2.GenerateAuthenticationKeyAsync(userAndUid.User, data).ConfigureAwait(false);

            if (authkeyResponse.IsOk())
            {
                return new(true, new(ComposeQueryString(data, authkeyResponse.Data, cultureOptions.LanguageCode)));
            }
            else
            {
                return new(false, SH.ServiceGachaLogUrlProviderAuthkeyRequestFailed);
            }
        }
        else
        {
            return new(false, SH.MustSelectUserAndUid);
        }
    }

    private static string ComposeQueryString(GenAuthKeyData genAuthKeyData, GameAuthKey gameAuthKey, string lang)
    {
        NameValueCollection collection = [];
        collection.Set("lang", lang);
        collection.Set("auth_appid", genAuthKeyData.AuthAppId);
        collection.Set("authkey", gameAuthKey.AuthKey);
        collection.Set("authkey_ver", $"{gameAuthKey.AuthKeyVersion:D}");
        collection.Set("sign_type", $"{gameAuthKey.SignType:D}");

        return collection.ToQueryString();
    }
}