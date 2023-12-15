// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

internal sealed class CardVerifiationHeaders
{
    public int ChallengeGame { get; private set; }

    public string ChallengePath { get; private set; } = string.Empty;

    public string Page { get; private set; } = string.Empty;

    public static CardVerifiationHeaders CreateForDailyNote()
    {
        return Create(ApiEndpoints.GameRecordDailyNotePath);
    }

    public static CardVerifiationHeaders CreateForIndex()
    {
        return Create(ApiEndpoints.GameRecordIndexPath);
    }

    public static CardVerifiationHeaders CreateForSpiralAbyss()
    {
        return Create(ApiEndpoints.GameRecordSpiralAbyssPath);
    }

    public static CardVerifiationHeaders CreateForCharacter()
    {
        return Create(ApiEndpoints.GameRecordCharacter, $"{HoyolabOptions.ToolVersion}_#/ys/role/all");
    }

    private static CardVerifiationHeaders Create(string path, string page = $"{HoyolabOptions.ToolVersion}_#/ys")
    {
        return new()
        {
            ChallengeGame = 2,
            ChallengePath = path,
            Page = page,
        };
    }

}