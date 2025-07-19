// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.HardChallenge;

internal sealed class ChallengeView
{
    private ChallengeView(HardChallengeChallenge challenge, HardChallengeMetadataContext context)
    {
        Name = challenge.Name;
        FormattedSeconds = SH.FormatViewModelHardChallengeSeconds(challenge.Seconds);
        Avatars = challenge.Team.SelectAsArray(AvatarView.Create, context);
        BestAvatars = challenge.BestAvatars.SelectAsArray(AvatarDamage.Create, context);
        Monster = MonsterView.Create(challenge.Monster, context);
    }

    public string Name { get; }

    public string FormattedSeconds { get; }

    public ImmutableArray<AvatarView> Avatars { get; }

    public ImmutableArray<AvatarDamage> BestAvatars { get; }

    public MonsterView Monster { get; }

    public static ChallengeView Create(HardChallengeChallenge challenge, HardChallengeMetadataContext context)
    {
        return new(challenge, context);
    }
}