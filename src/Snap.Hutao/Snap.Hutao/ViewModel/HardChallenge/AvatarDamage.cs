// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using System.Globalization;

namespace Snap.Hutao.ViewModel.HardChallenge;

internal sealed class AvatarDamage : AvatarView
{
    private AvatarDamage(HardChallengeBestAvatar avatar, Avatar metaAvatar)
        : base(metaAvatar)
    {
        Value = avatar.Dps;
        Type = avatar.Type.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture);
    }

    public int Value { get; }

    public string? Type { get; }

    public static AvatarDamage Create(HardChallengeBestAvatar avatar, HardChallengeMetadataContext context)
    {
        return new(avatar, context.GetAvatar(avatar.AvatarId));
    }

    public static AvatarDamage Create(HardChallengeBestAvatar avatar, Avatar metaAvatar)
    {
        return new(avatar, metaAvatar);
    }
}