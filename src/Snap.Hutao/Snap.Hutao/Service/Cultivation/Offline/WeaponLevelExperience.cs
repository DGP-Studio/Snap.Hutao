// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Cultivation.Offline;

internal static class WeaponLevelExperience
{
    private static ReadOnlySpan<int> FiveStarLevelExperienceData
    {
        get =>
        [
            000000, // Level 0 for padding
            000600, 000950, 001350, 001800, 002325, 002925, 003525, 004200, 004950, 005700,
            006525, 007400, 008300, 009225, 010200, 011250, 012300, 013425, 014600, 015750,
            017850, 019175, 020550, 021975, 023450, 024950, 026475, 028050, 029675, 031350,
            033050, 034800, 036575, 038400, 040250, 042150, 044100, 046100, 048125, 050150,
            054875, 057125, 059400, 061725, 064100, 066500, 068925, 071400, 073950, 076500,
            083075, 085850, 088650, 091550, 094425, 097400, 100350, 103400, 106475, 109575,
            118350, 121700, 125100, 128550, 132050, 135575, 139125, 142725, 146375, 150075,
            161525, 165500, 169500, 173550, 177650, 181800, 186000, 190250, 194525, 198875,
            234725, 263825, 296400, 332975, 373950, 419925, 471375, 529050, 593675, 000000,
        ];
    }

    private static ReadOnlySpan<int> FourStarLevelExperienceData
    {
        get =>
        [
            000000, // Level 0 for padding
            000400, 000625, 000900, 001200, 001550, 001950, 002350, 002800, 003300, 003800,
            004350, 004925, 005525, 006150, 006800, 007500, 008200, 008950, 009725, 010500,
            011900, 012775, 013700, 014650, 015625, 016625, 017650, 018700, 019775, 020900,
            022025, 023200, 024375, 025600, 026825, 028100, 029400, 030725, 032075, 033425,
            036575, 038075, 039600, 041150, 042725, 044325, 045950, 047600, 049300, 051000,
            055375, 057225, 059100, 061025, 062950, 064925, 066900, 068925, 070975, 073050,
            078900, 081125, 083400, 085700, 088025, 090375, 092750, 095150, 097575, 100050,
            107675, 110325, 113000, 115700, 118425, 121200, 124000, 126825, 129675, 132575,
            156475, 175875, 197600, 221975, 249300, 279950, 314250, 352700, 395775, 000000,
        ];
    }

    private static ReadOnlySpan<int> ThreeStarLevelExperienceData
    {
        get =>
        [
            000000, // Level 0 for padding
            000275, 000425, 000600, 000800, 001025, 001275, 001550, 001850, 002175, 002500,
            002875, 003250, 003650, 004050, 004500, 004950, 005400, 005900, 006425, 006925,
            007850, 008425, 009050, 009675, 010325, 010975, 011650, 012350, 013050, 013800,
            014525, 015300, 016100, 016900, 017700, 018550, 019400, 020275, 021175, 022050,
            024150, 025125, 026125, 027150, 028200, 029250, 030325, 031425, 032550, 033650,
            036550, 037775, 039000, 040275, 041550, 042850, 044150, 045500, 046850, 048225,
            052075, 053550, 055050, 056550, 058100, 059650, 061225, 062800, 064400, 066025,
            071075, 072825, 074575, 076350, 078150, 080000, 081850, 083700, 085575, 087500,
            103275, 116075, 130425, 146500, 164550, 184775, 207400, 232775, 261200, 000000,
        ];
    }

    private static ReadOnlySpan<int> TwoStarLevelExperienceData
    {
        get =>
        [
            00000, // Level 0 for padding
            00175, 00275, 00400, 00550, 00700, 00875, 01050, 01250, 01475, 01700,
            01950, 02225, 02475, 02775, 03050, 03375, 03700, 04025, 04375, 04725,
            05350, 05750, 06175, 06600, 07025, 07475, 07950, 08425, 08900, 09400,
            09900, 10450, 10975, 11525, 12075, 12650, 13225, 13825, 14425, 15050,
            16450, 17125, 17825, 18525, 19225, 19950, 20675, 21425, 22175, 22950,
            24925, 25750, 26600, 27450, 28325, 29225, 30100, 31025, 31950, 32875,
            35500, 36500, 37525, 38575, 39600, 40675, 41750, 42825, 43900, 00000,
        ];
    }

    private static ReadOnlySpan<int> OneStarLevelExperienceData
    {
        get =>
        [
            00000, // Level 0 for padding
            00125, 00200, 00275, 00350, 00475, 00575, 00700, 00850, 01000, 01150,
            01300, 01475, 01650, 01850, 02050, 02250, 02450, 02675, 02925, 03150,
            03575, 03825, 04100, 04400, 04700, 05000, 05300, 05600, 05925, 06275,
            06600, 06950, 07325, 07675, 08050, 08425, 08825, 09225, 09625, 10025,
            10975, 11425, 11875, 12350, 12825, 13300, 13775, 14275, 14800, 15300,
            16625, 17175, 17725, 18300, 18875, 19475, 20075, 20675, 21300, 21925,
            23675, 24350, 25025, 25700, 26400, 27125, 27825, 28550, 29275, 00000,
        ];
    }

    public static int CalculateTotalExperience(QualityType quality, int currentLevel, int targetLevel)
    {
        // Generally, these conditions should not be reached, but they are here for span safety.
        if (currentLevel >= targetLevel || targetLevel > GetMaxLevel(quality) || currentLevel < 1)
        {
            return 0;
        }

        ReadOnlySpan<int> levelExperienceData = quality switch
        {
            QualityType.QUALITY_ORANGE => FiveStarLevelExperienceData,
            QualityType.QUALITY_PURPLE => FourStarLevelExperienceData,
            QualityType.QUALITY_BLUE => ThreeStarLevelExperienceData,
            QualityType.QUALITY_GREEN => TwoStarLevelExperienceData,
            QualityType.QUALITY_WHITE => OneStarLevelExperienceData,
            _ => throw HutaoException.NotSupported(),
        };

        int totalExperience = 0;
        foreach (ref readonly int exp in levelExperienceData[currentLevel..targetLevel])
        {
            totalExperience += exp;
        }

        return totalExperience;
    }

    private static int GetMaxLevel(QualityType quality)
    {
        return quality switch
        {
            QualityType.QUALITY_WHITE or QualityType.QUALITY_GREEN => 70,
            QualityType.QUALITY_BLUE or QualityType.QUALITY_PURPLE or QualityType.QUALITY_ORANGE => 90,
            _ => throw HutaoException.NotSupported(),
        };
    }
}