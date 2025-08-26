// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Redeem;

internal enum RedeemStatus
{
    Ok,
    Invalid,
    NotExists,
    AlreadyUsed,
    Expired,
    NotEnough,
    NoSuchUser,
    DbError,
}