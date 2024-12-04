// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal interface IPassportMobileCaptchaProvider : IAigisProvider
{
    string? ActionType { get; }

    string? Mobile { get; }

    string? Captcha { get; }
}