// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal interface IPassportClient
{
    ValueTask<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default);

    ValueTask<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default);

    ValueTask<Response<UserInfoWrapper>> VerifyLtokenAsync(User user, CancellationToken token = default);

    ValueTask<Response<LoginResult>> LoginBySTokenAsync(Cookie stokenV1, CancellationToken token = default);

    ValueTask<Response<LoginResult>> LoginByGameTokenAsync(UidGameToken account, CancellationToken token = default);

    ValueTask<(string? Aigis, Response<MobileCaptcha> Response)> CreateLoginCaptchaAsync(string mobile, string? aigis, CancellationToken token = default);

    ValueTask<Response<LoginResult>> LoginByMobileCaptchaAsync(IPassportMobileCaptchaProvider provider, CancellationToken token = default);

    ValueTask<Response<LoginResult>> LoginByMobileCaptchaAsync(string actionType, string mobile, string captcha, string? aigis, CancellationToken token = default);
}