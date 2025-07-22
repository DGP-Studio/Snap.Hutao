// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Snap.Hutao.Benchmark;

[MemoryDiagnoser]
public class LevelExperienceBenchmark
{
    public IEnumerable<object[]> Parameters { get; } =
    [
        [1, 90],
    ];

    [Benchmark]
    [ArgumentsSource(nameof(Parameters))]
    public ImmutableArray<int> AvatarLevelExperienceRos(int currentLevel, int targetLevel)
    {
        ImmutableArray<int>.Builder results = ImmutableArray.CreateBuilder<int>();
        for (int i = currentLevel; i < targetLevel; i++)
        {
            results.Add(AvatarLevelExperienceUsingReadOnlySpan.CalculateTotalExperience(currentLevel, targetLevel));
        }

        return results.ToImmutable();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Parameters))]
    public ImmutableArray<int> AvatarLevelExperienceFDic(int currentLevel, int targetLevel)
    {
        ImmutableArray<int>.Builder results = ImmutableArray.CreateBuilder<int>();
        for (int i = currentLevel; i < targetLevel; i++)
        {
            results.Add(AvatarLevelExperienceUsingFrozenDictionary.CalculateTotalExperience(currentLevel, targetLevel));
        }

        return results.ToImmutable();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Parameters))]
    public ImmutableArray<int> WeaponLevelExperienceRos(int currentLevel, int targetLevel)
    {
        QualityType quality = QualityType.QUALITY_ORANGE;
        ImmutableArray<int>.Builder results = ImmutableArray.CreateBuilder<int>();
        for (int i = currentLevel; i < targetLevel; i++)
        {
            results.Add(WeaponLevelExperienceUsingReadOnlySpan.CalculateTotalExperience(quality, currentLevel, targetLevel));
        }

        return results.ToImmutable();
    }

    [Benchmark]
    [ArgumentsSource(nameof(Parameters))]
    public ImmutableArray<int> WeaponLevelExperienceFDic(int currentLevel, int targetLevel)
    {
        QualityType quality = QualityType.QUALITY_ORANGE;
        ImmutableArray<int>.Builder results = ImmutableArray.CreateBuilder<int>();
        for (int i = currentLevel; i < targetLevel; i++)
        {
            results.Add(WeaponLevelExperienceUsingFrozenDictionary.CalculateTotalExperience(quality, currentLevel, targetLevel));
        }

        return results.ToImmutable();
    }
}