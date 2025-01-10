// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Item;

internal sealed class MaterialDropDistribution
{
    public const double MoraPerResin = 50D;

    public static readonly MaterialDropDistribution Nine = new(9, 60000D, 122500D, 10.12, 2375D, 16.708, 2200D, 3D, 6000D, 2.4, 8100D);
    public static readonly MaterialDropDistribution Eight = new(8, 60000D, 122500D, 10.12, 2375D, 16.708, 2200D, 2.56, 6000D, 2.4, 8100D);
    public static readonly MaterialDropDistribution Seven = new(7, 60000D, 122500D, 10.12, 2375D, 16.708, 2200D, 2.56, 5200D, 2.1, 8100D);
    public static readonly MaterialDropDistribution Six = new(6, 60000D, 122500D, 10.12, 2375D, 16.708, 2200D, 2.2, 4725D, 1.55, 8000D);
    public static readonly MaterialDropDistribution Five = new(5, 52000D, 102500D, 7.8, 2050D, 16.708, 2200D, 2.2, 4450D, 1D, 7600D);

    private MaterialDropDistribution(int worldLevel, double blossomOfWealth, double blossomOfRevelation, double talentBooks, double talentBooksMora, double weaponAscension, double weaponAscensionMora, double normalBoss, double normalBossMora, double weeklyBoss, double weeklyBossMora)
    {
        WorldLevel = worldLevel;
        BlossomOfWealth = blossomOfWealth + (20 * MoraPerResin);
        BlossomOfRevelation = blossomOfRevelation;
        TalentBooks = talentBooks;
        TalentBooksMora = talentBooksMora;
        WeaponAscension = weaponAscension;
        WeaponAscensionMora = weaponAscensionMora;
        NormalBoss = normalBoss;
        NormalBossMora = normalBossMora;
        WeeklyBoss = weeklyBoss;
        WeeklyBossMora = weeklyBossMora;
    }

    public static ImmutableArray<MaterialDropDistribution> Distributions { get; } = [Five, Six, Seven, Eight, Nine];

    public int WorldLevel { get; }

    public double BlossomOfWealth { get; }

    public double BlossomOfRevelation { get; }

    public double TalentBooks { get; }

    public double TalentBooksMora { get; }

    public double WeaponAscension { get; }

    public double WeaponAscensionMora { get; }

    public double NormalBoss { get; }

    public double NormalBossMora { get; }

    public double WeeklyBoss { get; }

    public double WeeklyBossMora { get; }
}