// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal interface IPassportPasswordProvider
{
    string? Account { get; }

    string? Password { get; }

    string? Aigis { get; }
}