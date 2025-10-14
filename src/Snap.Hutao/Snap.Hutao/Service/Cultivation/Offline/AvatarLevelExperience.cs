// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Cultivation.Offline;

internal static class AvatarLevelExperience
{
    // AvatarLevelExcelConfigData
    private static ReadOnlySpan<int> LevelExperienceData
    {
        get =>
        [
            000000, // Level 0 for padding
            001000, 001325, 001700, 002150, 002625, 003150, 003725, 004350, 005000, 005700,
            006450, 007225, 008050, 008925, 009825, 010750, 011725, 012725, 013775, 014875,
            016800, 018000, 019250, 020550, 021875, 023250, 024650, 026100, 027575, 029100,
            030650, 032250, 033875, 035550, 037250, 038975, 040750, 042575, 044425, 046300,
            050625, 052700, 054775, 056900, 059075, 061275, 063525, 065800, 068125, 070475,
            076500, 079050, 081650, 084275, 086950, 089650, 092400, 095175, 098000, 100875,
            108950, 112050, 115175, 118325, 121525, 124775, 128075, 131400, 134775, 138175,
            148700, 152375, 156075, 159825, 163600, 167425, 171300, 175225, 179175, 183175,
            216225, 243025, 273100, 306800, 344600, 386950, 434425, 487625, 547200, 000000
        ];
    }

    public static int CalculateTotalExperience(int currentLevel, int targetLevel)
    {
        if (currentLevel >= targetLevel || targetLevel > 90 || currentLevel < 1)
        {
            return 0;
        }

        int totalExperience = 0;
        foreach (ref readonly int exp in LevelExperienceData[currentLevel..targetLevel])
        {
            totalExperience += exp;
        }

        return totalExperience;
    }
}