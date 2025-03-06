// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class UserInfo
{
    public required string NormalizedUserName { get; init; }

    public required string UserName { get; init; }

    public required bool IsLicensedDeveloper { get; init; }

    public required bool IsMaintainer { get; init; }

    public required DateTimeOffset GachaLogExpireAt { get; init; }

    public required DateTimeOffset CdnExpireAt { get; init; }
}