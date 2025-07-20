using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace Snap.Hutao.Benchmark;

[MemoryDiagnoser]
public class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<Program>();
    }

    public IEnumerable<object[]> Parameters { get; } =
    [
        [1, 90],
    ];

    [Benchmark]
    [ArgumentsSource(nameof(Parameters))]
    public int AvatarLevelExperienceRos(int currentLevel, int targetLevel)
    {
        return AvatarLevelExperienceUsingReadOnlySpan.CalculateTotalExperience(currentLevel, targetLevel);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Parameters))]
    public int AvatarLevelExperienceFDic(int currentLevel, int targetLevel)
    {
        return AvatarLevelExperienceUsingFrozenDictionary.CalculateTotalExperience(currentLevel, targetLevel);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Parameters))]
    public int WeaponLevelExperienceRos(int currentLevel, int targetLevel)
    {
        QualityType quality = QualityType.QUALITY_ORANGE;
        return WeaponLevelExperienceUsingReadOnlySpan.CalculateTotalExperience(quality, currentLevel, targetLevel);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Parameters))]
    public int WeaponLevelExperienceFDic(int currentLevel, int targetLevel)
    {
        QualityType quality = QualityType.QUALITY_ORANGE;
        return WeaponLevelExperienceUsingFrozenDictionary.CalculateTotalExperience(quality, currentLevel, targetLevel);
    }
}

internal static class AvatarLevelExperienceUsingReadOnlySpan
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
        // Generally, these conditions should not be reached, but they are here for span safety.
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

internal static class WeaponLevelExperienceUsingReadOnlySpan
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
            _ => throw new InvalidOperationException(),
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
            _ => throw new InvalidOperationException(),
        };
    }
}

internal static class WeaponLevelExperienceUsingFrozenDictionary
{
    private static readonly FrozenDictionary<int, int> FiveStarLevelExperienceData = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(1, 600),
        KeyValuePair.Create(2, 950),
        KeyValuePair.Create(3, 1350),
        KeyValuePair.Create(4, 1800),
        KeyValuePair.Create(5, 2325),
        KeyValuePair.Create(6, 2925),
        KeyValuePair.Create(7, 3525),
        KeyValuePair.Create(8, 4200),
        KeyValuePair.Create(9, 4950),
        KeyValuePair.Create(10, 5700),
        KeyValuePair.Create(11, 6525),
        KeyValuePair.Create(12, 7400),
        KeyValuePair.Create(13, 8300),
        KeyValuePair.Create(14, 9225),
        KeyValuePair.Create(15, 10200),
        KeyValuePair.Create(16, 11250),
        KeyValuePair.Create(17, 12300),
        KeyValuePair.Create(18, 13425),
        KeyValuePair.Create(19, 14600),
        KeyValuePair.Create(20, 15750),
        KeyValuePair.Create(21, 17850),
        KeyValuePair.Create(22, 19175),
        KeyValuePair.Create(23, 20550),
        KeyValuePair.Create(24, 21975),
        KeyValuePair.Create(25, 23450),
        KeyValuePair.Create(26, 24950),
        KeyValuePair.Create(27, 26475),
        KeyValuePair.Create(28, 28050),
        KeyValuePair.Create(29, 29675),
        KeyValuePair.Create(30, 31350),
        KeyValuePair.Create(31, 33050),
        KeyValuePair.Create(32, 34800),
        KeyValuePair.Create(33, 36575),
        KeyValuePair.Create(34, 38400),
        KeyValuePair.Create(35, 40250),
        KeyValuePair.Create(36, 42150),
        KeyValuePair.Create(37, 44100),
        KeyValuePair.Create(38, 46100),
        KeyValuePair.Create(39, 48125),
        KeyValuePair.Create(40, 50150),
        KeyValuePair.Create(41, 54875),
        KeyValuePair.Create(42, 57125),
        KeyValuePair.Create(43, 59400),
        KeyValuePair.Create(44, 61725),
        KeyValuePair.Create(45, 64100),
        KeyValuePair.Create(46, 66500),
        KeyValuePair.Create(47, 68925),
        KeyValuePair.Create(48, 71400),
        KeyValuePair.Create(49, 73950),
        KeyValuePair.Create(50, 76500),
        KeyValuePair.Create(51, 83075),
        KeyValuePair.Create(52, 85850),
        KeyValuePair.Create(53, 88650),
        KeyValuePair.Create(54, 91550),
        KeyValuePair.Create(55, 94425),
        KeyValuePair.Create(56, 97400),
        KeyValuePair.Create(57, 100350),
        KeyValuePair.Create(58, 103400),
        KeyValuePair.Create(59, 106475),
        KeyValuePair.Create(60, 109575),
        KeyValuePair.Create(61, 118350),
        KeyValuePair.Create(62, 121700),
        KeyValuePair.Create(63, 125100),
        KeyValuePair.Create(64, 128550),
        KeyValuePair.Create(65, 132050),
        KeyValuePair.Create(66, 135575),
        KeyValuePair.Create(67, 139125),
        KeyValuePair.Create(68, 142725),
        KeyValuePair.Create(69, 146375),
        KeyValuePair.Create(70, 150075),
        KeyValuePair.Create(71, 161525),
        KeyValuePair.Create(72, 165500),
        KeyValuePair.Create(73, 169500),
        KeyValuePair.Create(74, 173550),
        KeyValuePair.Create(75, 177650),
        KeyValuePair.Create(76, 181800),
        KeyValuePair.Create(77, 186000),
        KeyValuePair.Create(78, 190250),
        KeyValuePair.Create(79, 194525),
        KeyValuePair.Create(80, 198875),
        KeyValuePair.Create(81, 234725),
        KeyValuePair.Create(82, 263825),
        KeyValuePair.Create(83, 296400),
        KeyValuePair.Create(84, 332975),
        KeyValuePair.Create(85, 373950),
        KeyValuePair.Create(86, 419925),
        KeyValuePair.Create(87, 471375),
        KeyValuePair.Create(88, 529050),
        KeyValuePair.Create(89, 593675),
        KeyValuePair.Create(90, 0),
    ]);

    private static readonly FrozenDictionary<int, int> FourStarLevelExperienceData = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(1, 400),
        KeyValuePair.Create(2, 625),
        KeyValuePair.Create(3, 900),
        KeyValuePair.Create(4, 1200),
        KeyValuePair.Create(5, 1550),
        KeyValuePair.Create(6, 1950),
        KeyValuePair.Create(7, 2350),
        KeyValuePair.Create(8, 2800),
        KeyValuePair.Create(9, 3300),
        KeyValuePair.Create(10, 3800),
        KeyValuePair.Create(11, 4350),
        KeyValuePair.Create(12, 4925),
        KeyValuePair.Create(13, 5525),
        KeyValuePair.Create(14, 6150),
        KeyValuePair.Create(15, 6800),
        KeyValuePair.Create(16, 7500),
        KeyValuePair.Create(17, 8200),
        KeyValuePair.Create(18, 8950),
        KeyValuePair.Create(19, 9725),
        KeyValuePair.Create(20, 10500),
        KeyValuePair.Create(21, 11900),
        KeyValuePair.Create(22, 12775),
        KeyValuePair.Create(23, 13700),
        KeyValuePair.Create(24, 14650),
        KeyValuePair.Create(25, 15625),
        KeyValuePair.Create(26, 16625),
        KeyValuePair.Create(27, 17650),
        KeyValuePair.Create(28, 18700),
        KeyValuePair.Create(29, 19775),
        KeyValuePair.Create(30, 20900),
        KeyValuePair.Create(31, 22025),
        KeyValuePair.Create(32, 23200),
        KeyValuePair.Create(33, 24375),
        KeyValuePair.Create(34, 25600),
        KeyValuePair.Create(35, 26825),
        KeyValuePair.Create(36, 28100),
        KeyValuePair.Create(37, 29400),
        KeyValuePair.Create(38, 30725),
        KeyValuePair.Create(39, 32075),
        KeyValuePair.Create(40, 33425),
        KeyValuePair.Create(41, 36575),
        KeyValuePair.Create(42, 38075),
        KeyValuePair.Create(43, 39600),
        KeyValuePair.Create(44, 41150),
        KeyValuePair.Create(45, 42725),
        KeyValuePair.Create(46, 44325),
        KeyValuePair.Create(47, 45950),
        KeyValuePair.Create(48, 47600),
        KeyValuePair.Create(49, 49300),
        KeyValuePair.Create(50, 51000),
        KeyValuePair.Create(51, 55375),
        KeyValuePair.Create(52, 57225),
        KeyValuePair.Create(53, 59100),
        KeyValuePair.Create(54, 61025),
        KeyValuePair.Create(55, 62950),
        KeyValuePair.Create(56, 64925),
        KeyValuePair.Create(57, 66900),
        KeyValuePair.Create(58, 68925),
        KeyValuePair.Create(59, 70975),
        KeyValuePair.Create(60, 73050),
        KeyValuePair.Create(61, 78900),
        KeyValuePair.Create(62, 81125),
        KeyValuePair.Create(63, 83400),
        KeyValuePair.Create(64, 85700),
        KeyValuePair.Create(65, 88025),
        KeyValuePair.Create(66, 90375),
        KeyValuePair.Create(67, 92750),
        KeyValuePair.Create(68, 95150),
        KeyValuePair.Create(69, 97575),
        KeyValuePair.Create(70, 100050),
        KeyValuePair.Create(71, 107675),
        KeyValuePair.Create(72, 110325),
        KeyValuePair.Create(73, 113000),
        KeyValuePair.Create(74, 115700),
        KeyValuePair.Create(75, 118425),
        KeyValuePair.Create(76, 121200),
        KeyValuePair.Create(77, 124000),
        KeyValuePair.Create(78, 126825),
        KeyValuePair.Create(79, 129675),
        KeyValuePair.Create(80, 132575),
        KeyValuePair.Create(81, 156475),
        KeyValuePair.Create(82, 175875),
        KeyValuePair.Create(83, 197600),
        KeyValuePair.Create(84, 221975),
        KeyValuePair.Create(85, 249300),
        KeyValuePair.Create(86, 279950),
        KeyValuePair.Create(87, 314250),
        KeyValuePair.Create(88, 352700),
        KeyValuePair.Create(89, 395775),
        KeyValuePair.Create(90, 0),
    ]);

    private static readonly FrozenDictionary<int, int> ThreeStarLevelExperienceData = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(1, 275),
        KeyValuePair.Create(2, 425),
        KeyValuePair.Create(3, 600),
        KeyValuePair.Create(4, 800),
        KeyValuePair.Create(5, 1025),
        KeyValuePair.Create(6, 1275),
        KeyValuePair.Create(7, 1550),
        KeyValuePair.Create(8, 1850),
        KeyValuePair.Create(9, 2175),
        KeyValuePair.Create(10, 2500),
        KeyValuePair.Create(11, 2875),
        KeyValuePair.Create(12, 3250),
        KeyValuePair.Create(13, 3650),
        KeyValuePair.Create(14, 4050),
        KeyValuePair.Create(15, 4500),
        KeyValuePair.Create(16, 4950),
        KeyValuePair.Create(17, 5400),
        KeyValuePair.Create(18, 5900),
        KeyValuePair.Create(19, 6425),
        KeyValuePair.Create(20, 6925),
        KeyValuePair.Create(21, 7850),
        KeyValuePair.Create(22, 8425),
        KeyValuePair.Create(23, 9050),
        KeyValuePair.Create(24, 9675),
        KeyValuePair.Create(25, 10325),
        KeyValuePair.Create(26, 10975),
        KeyValuePair.Create(27, 11650),
        KeyValuePair.Create(28, 12350),
        KeyValuePair.Create(29, 13050),
        KeyValuePair.Create(30, 13800),
        KeyValuePair.Create(31, 14525),
        KeyValuePair.Create(32, 15300),
        KeyValuePair.Create(33, 16100),
        KeyValuePair.Create(34, 16900),
        KeyValuePair.Create(35, 17700),
        KeyValuePair.Create(36, 18550),
        KeyValuePair.Create(37, 19400),
        KeyValuePair.Create(38, 20275),
        KeyValuePair.Create(39, 21175),
        KeyValuePair.Create(40, 22050),
        KeyValuePair.Create(41, 24150),
        KeyValuePair.Create(42, 25125),
        KeyValuePair.Create(43, 26125),
        KeyValuePair.Create(44, 27150),
        KeyValuePair.Create(45, 28200),
        KeyValuePair.Create(46, 29250),
        KeyValuePair.Create(47, 30325),
        KeyValuePair.Create(48, 31425),
        KeyValuePair.Create(49, 32550),
        KeyValuePair.Create(50, 33650),
        KeyValuePair.Create(51, 36550),
        KeyValuePair.Create(52, 37775),
        KeyValuePair.Create(53, 39000),
        KeyValuePair.Create(54, 40275),
        KeyValuePair.Create(55, 41550),
        KeyValuePair.Create(56, 42850),
        KeyValuePair.Create(57, 44150),
        KeyValuePair.Create(58, 45500),
        KeyValuePair.Create(59, 46850),
        KeyValuePair.Create(60, 48225),
        KeyValuePair.Create(61, 52075),
        KeyValuePair.Create(62, 53550),
        KeyValuePair.Create(63, 55050),
        KeyValuePair.Create(64, 56550),
        KeyValuePair.Create(65, 58100),
        KeyValuePair.Create(66, 59650),
        KeyValuePair.Create(67, 61225),
        KeyValuePair.Create(68, 62800),
        KeyValuePair.Create(69, 64400),
        KeyValuePair.Create(70, 66025),
        KeyValuePair.Create(71, 71075),
        KeyValuePair.Create(72, 72825),
        KeyValuePair.Create(73, 74575),
        KeyValuePair.Create(74, 76350),
        KeyValuePair.Create(75, 78150),
        KeyValuePair.Create(76, 80000),
        KeyValuePair.Create(77, 81850),
        KeyValuePair.Create(78, 83700),
        KeyValuePair.Create(79, 85575),
        KeyValuePair.Create(80, 87500),
        KeyValuePair.Create(81, 103275),
        KeyValuePair.Create(82, 116075),
        KeyValuePair.Create(83, 130425),
        KeyValuePair.Create(84, 146500),
        KeyValuePair.Create(85, 164550),
        KeyValuePair.Create(86, 184775),
        KeyValuePair.Create(87, 207400),
        KeyValuePair.Create(88, 232775),
        KeyValuePair.Create(89, 261200),
        KeyValuePair.Create(90, 0),
    ]);

    private static readonly FrozenDictionary<int, int> TwoStarLevelExperienceData = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(1, 175),
        KeyValuePair.Create(2, 275),
        KeyValuePair.Create(3, 400),
        KeyValuePair.Create(4, 550),
        KeyValuePair.Create(5, 700),
        KeyValuePair.Create(6, 875),
        KeyValuePair.Create(7, 1050),
        KeyValuePair.Create(8, 1250),
        KeyValuePair.Create(9, 1475),
        KeyValuePair.Create(10, 1700),
        KeyValuePair.Create(11, 1950),
        KeyValuePair.Create(12, 2225),
        KeyValuePair.Create(13, 2475),
        KeyValuePair.Create(14, 2775),
        KeyValuePair.Create(15, 3050),
        KeyValuePair.Create(16, 3375),
        KeyValuePair.Create(17, 3700),
        KeyValuePair.Create(18, 4025),
        KeyValuePair.Create(19, 4375),
        KeyValuePair.Create(20, 4725),
        KeyValuePair.Create(21, 5350),
        KeyValuePair.Create(22, 5750),
        KeyValuePair.Create(23, 6175),
        KeyValuePair.Create(24, 6600),
        KeyValuePair.Create(25, 7025),
        KeyValuePair.Create(26, 7475),
        KeyValuePair.Create(27, 7950),
        KeyValuePair.Create(28, 8425),
        KeyValuePair.Create(29, 8900),
        KeyValuePair.Create(30, 9400),
        KeyValuePair.Create(31, 9900),
        KeyValuePair.Create(32, 10450),
        KeyValuePair.Create(33, 10975),
        KeyValuePair.Create(34, 11525),
        KeyValuePair.Create(35, 12075),
        KeyValuePair.Create(36, 12650),
        KeyValuePair.Create(37, 13225),
        KeyValuePair.Create(38, 13825),
        KeyValuePair.Create(39, 14425),
        KeyValuePair.Create(40, 15050),
        KeyValuePair.Create(41, 16450),
        KeyValuePair.Create(42, 17125),
        KeyValuePair.Create(43, 17825),
        KeyValuePair.Create(44, 18525),
        KeyValuePair.Create(45, 19225),
        KeyValuePair.Create(46, 19950),
        KeyValuePair.Create(47, 20675),
        KeyValuePair.Create(48, 21425),
        KeyValuePair.Create(49, 22175),
        KeyValuePair.Create(50, 22950),
        KeyValuePair.Create(51, 24925),
        KeyValuePair.Create(52, 25750),
        KeyValuePair.Create(53, 26600),
        KeyValuePair.Create(54, 27450),
        KeyValuePair.Create(55, 28325),
        KeyValuePair.Create(56, 29225),
        KeyValuePair.Create(57, 30100),
        KeyValuePair.Create(58, 31025),
        KeyValuePair.Create(59, 31950),
        KeyValuePair.Create(60, 32875),
        KeyValuePair.Create(61, 35500),
        KeyValuePair.Create(62, 36500),
        KeyValuePair.Create(63, 37525),
        KeyValuePair.Create(64, 38575),
        KeyValuePair.Create(65, 39600),
        KeyValuePair.Create(66, 40675),
        KeyValuePair.Create(67, 41750),
        KeyValuePair.Create(68, 42825),
        KeyValuePair.Create(69, 43900),
        KeyValuePair.Create(70, 0),
    ]);

    private static readonly FrozenDictionary<int, int> OneStarLevelExperienceData = FrozenDictionary.ToFrozenDictionary(
    [
        KeyValuePair.Create(1, 125),
        KeyValuePair.Create(2, 200),
        KeyValuePair.Create(3, 275),
        KeyValuePair.Create(4, 350),
        KeyValuePair.Create(5, 475),
        KeyValuePair.Create(6, 575),
        KeyValuePair.Create(7, 700),
        KeyValuePair.Create(8, 850),
        KeyValuePair.Create(9, 1000),
        KeyValuePair.Create(10, 1150),
        KeyValuePair.Create(11, 1300),
        KeyValuePair.Create(12, 1475),
        KeyValuePair.Create(13, 1650),
        KeyValuePair.Create(14, 1850),
        KeyValuePair.Create(15, 2050),
        KeyValuePair.Create(16, 2250),
        KeyValuePair.Create(17, 2450),
        KeyValuePair.Create(18, 2675),
        KeyValuePair.Create(19, 2925),
        KeyValuePair.Create(20, 3150),
        KeyValuePair.Create(21, 3575),
        KeyValuePair.Create(22, 3825),
        KeyValuePair.Create(23, 4100),
        KeyValuePair.Create(24, 4400),
        KeyValuePair.Create(25, 4700),
        KeyValuePair.Create(26, 5000),
        KeyValuePair.Create(27, 5300),
        KeyValuePair.Create(28, 5600),
        KeyValuePair.Create(29, 5925),
        KeyValuePair.Create(30, 6275),
        KeyValuePair.Create(31, 6600),
        KeyValuePair.Create(32, 6950),
        KeyValuePair.Create(33, 7325),
        KeyValuePair.Create(34, 7675),
        KeyValuePair.Create(35, 8050),
        KeyValuePair.Create(36, 8425),
        KeyValuePair.Create(37, 8825),
        KeyValuePair.Create(38, 9225),
        KeyValuePair.Create(39, 9625),
        KeyValuePair.Create(40, 10025),
        KeyValuePair.Create(41, 10975),
        KeyValuePair.Create(42, 11425),
        KeyValuePair.Create(43, 11875),
        KeyValuePair.Create(44, 12350),
        KeyValuePair.Create(45, 12825),
        KeyValuePair.Create(46, 13300),
        KeyValuePair.Create(47, 13775),
        KeyValuePair.Create(48, 14275),
        KeyValuePair.Create(49, 14800),
        KeyValuePair.Create(50, 15300),
        KeyValuePair.Create(51, 16625),
        KeyValuePair.Create(52, 17175),
        KeyValuePair.Create(53, 17725),
        KeyValuePair.Create(54, 18300),
        KeyValuePair.Create(55, 18875),
        KeyValuePair.Create(56, 19475),
        KeyValuePair.Create(57, 20075),
        KeyValuePair.Create(58, 20675),
        KeyValuePair.Create(59, 21300),
        KeyValuePair.Create(60, 21925),
        KeyValuePair.Create(61, 23675),
        KeyValuePair.Create(62, 24350),
        KeyValuePair.Create(63, 25025),
        KeyValuePair.Create(64, 25700),
        KeyValuePair.Create(65, 26400),
        KeyValuePair.Create(66, 27125),
        KeyValuePair.Create(67, 27825),
        KeyValuePair.Create(68, 28550),
        KeyValuePair.Create(69, 29275),
        KeyValuePair.Create(70, 0),
    ]);

    public static int GetExperienceToNextLevel(QualityType quality, int level)
    {
        if (level >= GetMaxLevel(quality))
        {
            return 0;
        }

        FrozenDictionary<int, int> levelData = quality switch
        {
            QualityType.QUALITY_ORANGE => FiveStarLevelExperienceData,
            QualityType.QUALITY_PURPLE => FourStarLevelExperienceData,
            QualityType.QUALITY_BLUE => ThreeStarLevelExperienceData,
            QualityType.QUALITY_GREEN => TwoStarLevelExperienceData,
            QualityType.QUALITY_WHITE => OneStarLevelExperienceData,
            _ => throw new InvalidOperationException(),
        };

        return levelData.TryGetValue(level, out int experience) ? experience : 0;
    }

    public static int CalculateTotalExperience(QualityType quality, int currentLevel, int targetLevel)
    {
        int maxLevel = GetMaxLevel(quality);
        if (currentLevel >= targetLevel || currentLevel >= maxLevel)
        {
            return 0;
        }

        int totalExperience = 0;
        for (int level = currentLevel; level < targetLevel && level < maxLevel; level++)
        {
            totalExperience += GetExperienceToNextLevel(quality, level);
        }

        return totalExperience;
    }

    private static int GetMaxLevel(QualityType quality)
    {
        return quality switch
        {
            QualityType.QUALITY_WHITE or QualityType.QUALITY_GREEN => 70,
            QualityType.QUALITY_BLUE or QualityType.QUALITY_PURPLE or QualityType.QUALITY_ORANGE => 90,
            _ => throw new InvalidOperationException(),
        };
    }
}

internal enum QualityType
{
    /// <summary>
    /// 无
    /// </summary>
    QUALITY_NONE = 0,
    QUALITY_WHITE = 1,
    QUALITY_GREEN = 2,
    QUALITY_BLUE = 3,
    QUALITY_PURPLE = 4,
    QUALITY_ORANGE = 5,
    QUALITY_ORANGE_SP = 105,
}