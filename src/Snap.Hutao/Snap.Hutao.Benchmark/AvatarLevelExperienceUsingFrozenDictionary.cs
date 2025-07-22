// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;
using System.Collections.Generic;

namespace Snap.Hutao.Benchmark;

internal static class AvatarLevelExperienceUsingFrozenDictionary
{
    private const int MaxLevel = 90;

    private static readonly FrozenDictionary<int, int> LevelExperienceData = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(1, 1000),
        KeyValuePair.Create(2, 1325),
        KeyValuePair.Create(3, 1700),
        KeyValuePair.Create(4, 2150),
        KeyValuePair.Create(5, 2625),
        KeyValuePair.Create(6, 3150),
        KeyValuePair.Create(7, 3725),
        KeyValuePair.Create(8, 4350),
        KeyValuePair.Create(9, 5000),
        KeyValuePair.Create(10, 5700),
        KeyValuePair.Create(11, 6450),
        KeyValuePair.Create(12, 7225),
        KeyValuePair.Create(13, 8050),
        KeyValuePair.Create(14, 8925),
        KeyValuePair.Create(15, 9825),
        KeyValuePair.Create(16, 10750),
        KeyValuePair.Create(17, 11725),
        KeyValuePair.Create(18, 12725),
        KeyValuePair.Create(19, 13775),
        KeyValuePair.Create(20, 14875),
        KeyValuePair.Create(21, 16800),
        KeyValuePair.Create(22, 18000),
        KeyValuePair.Create(23, 19250),
        KeyValuePair.Create(24, 20550),
        KeyValuePair.Create(25, 21875),
        KeyValuePair.Create(26, 23250),
        KeyValuePair.Create(27, 24650),
        KeyValuePair.Create(28, 26100),
        KeyValuePair.Create(29, 27575),
        KeyValuePair.Create(30, 29100),
        KeyValuePair.Create(31, 30650),
        KeyValuePair.Create(32, 32250),
        KeyValuePair.Create(33, 33875),
        KeyValuePair.Create(34, 35550),
        KeyValuePair.Create(35, 37250),
        KeyValuePair.Create(36, 38975),
        KeyValuePair.Create(37, 40750),
        KeyValuePair.Create(38, 42575),
        KeyValuePair.Create(39, 44425),
        KeyValuePair.Create(40, 46300),
        KeyValuePair.Create(41, 50625),
        KeyValuePair.Create(42, 52700),
        KeyValuePair.Create(43, 54775),
        KeyValuePair.Create(44, 56900),
        KeyValuePair.Create(45, 59075),
        KeyValuePair.Create(46, 61275),
        KeyValuePair.Create(47, 63525),
        KeyValuePair.Create(48, 65800),
        KeyValuePair.Create(49, 68125),
        KeyValuePair.Create(50, 70475),
        KeyValuePair.Create(51, 76500),
        KeyValuePair.Create(52, 79050),
        KeyValuePair.Create(53, 81650),
        KeyValuePair.Create(54, 84275),
        KeyValuePair.Create(55, 86950),
        KeyValuePair.Create(56, 89650),
        KeyValuePair.Create(57, 92400),
        KeyValuePair.Create(58, 95175),
        KeyValuePair.Create(59, 98000),
        KeyValuePair.Create(60, 100875),
        KeyValuePair.Create(61, 108950),
        KeyValuePair.Create(62, 112050),
        KeyValuePair.Create(63, 115175),
        KeyValuePair.Create(64, 118325),
        KeyValuePair.Create(65, 121525),
        KeyValuePair.Create(66, 124775),
        KeyValuePair.Create(67, 128075),
        KeyValuePair.Create(68, 131400),
        KeyValuePair.Create(69, 134775),
        KeyValuePair.Create(70, 138175),
        KeyValuePair.Create(71, 148700),
        KeyValuePair.Create(72, 152375),
        KeyValuePair.Create(73, 156075),
        KeyValuePair.Create(74, 159825),
        KeyValuePair.Create(75, 163600),
        KeyValuePair.Create(76, 167425),
        KeyValuePair.Create(77, 171300),
        KeyValuePair.Create(78, 175225),
        KeyValuePair.Create(79, 179175),
        KeyValuePair.Create(80, 183175),
        KeyValuePair.Create(81, 216225),
        KeyValuePair.Create(82, 243025),
        KeyValuePair.Create(83, 273100),
        KeyValuePair.Create(84, 306800),
        KeyValuePair.Create(85, 344600),
        KeyValuePair.Create(86, 386950),
        KeyValuePair.Create(87, 434425),
        KeyValuePair.Create(88, 487625),
        KeyValuePair.Create(89, 547200),
        KeyValuePair.Create(90, 0), // Level 90 does not require experience to level up, as it is the maximum level.
    ]);

    public static int GetExperienceToNextLevel(int level)
    {
        if (level >= MaxLevel)
        {
            return 0;
        }

        return LevelExperienceData.TryGetValue(level, out int experience) ? experience : 0;
    }

    public static int CalculateTotalExperience(int currentLevel, int targetLevel)
    {
        if (currentLevel >= targetLevel || currentLevel >= MaxLevel)
        {
            return 0;
        }

        int totalExperience = 0;
        for (int level = currentLevel; level < targetLevel && level < MaxLevel; level++)
        {
            totalExperience += GetExperienceToNextLevel(level);
        }

        return totalExperience;
    }
}