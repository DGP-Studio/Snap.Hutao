// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Item;

internal sealed class MaterialDropDistribution
{
    public static readonly MaterialDropDistribution Nine = new(9, 60000D, 122500D, 10.12, 16.708, 3, 2.4);
    public static readonly MaterialDropDistribution Eight = new(8, 60000D, 122500D, 10.12, 16.708, 2.56, 2.4);
    public static readonly MaterialDropDistribution Seven = new(7, 60000D, 122500D, 10.12, 16.708, 2.56, 2.1);
    public static readonly MaterialDropDistribution Six = new(6, 60000D, 122500D, 10.12, 16.708, 2.2, 1.55);
    public static readonly MaterialDropDistribution Five = new(5, 52000D, 102500D, 7.8, 16.708, 2.2, 1D);

    private MaterialDropDistribution(int worldLevel, double blossomOfWealth, double blossomOfRevelation, double talentBooks, double weaponAscension, double normalBoss, double weeklyBoss)
    {
        WorldLevel = worldLevel;
        BlossomOfWealth = blossomOfWealth;
        BlossomOfRevelation = blossomOfRevelation;
        TalentBooks = talentBooks;
        WeaponAscension = weaponAscension;
        NormalBoss = normalBoss;
        WeeklyBoss = weeklyBoss;
    }

    public static ImmutableArray<MaterialDropDistribution> Distributions { get; } = [Five, Six, Seven, Eight, Nine];

    public int WorldLevel { get; }

    public double BlossomOfWealth { get; }

    public double BlossomOfRevelation { get; }

    public double TalentBooks { get; }

    public double WeaponAscension { get; }

    public double NormalBoss { get; }

    public double WeeklyBoss { get; }
}