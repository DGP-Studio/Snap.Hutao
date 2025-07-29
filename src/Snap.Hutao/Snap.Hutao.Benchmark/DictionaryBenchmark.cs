// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Benchmark;

[MemoryDiagnoser]
[HardwareCounters(HardwareCounter.CacheMisses, HardwareCounter.BranchMispredictions)]
public class DictionaryBenchmark
{
    private Dictionary<int, int> _dictionary = null!;

    public IEnumerable<Dictionary<int, int>> Parameters => new[]
    {
        Enumerable.Range(1, 10).ToDictionary(i => i, i => i),
        Enumerable.Range(1, 1000).ToDictionary(i => i, i => i),
        new Dictionary<int, int>() // 空字典
    };

    [ParamsSource(nameof(Parameters))]
    public Dictionary<int, int> Input { get; set; } = null!;

    [GlobalSetup(Targets = [nameof(TryGetValue), nameof(CollectionsMarshalTryGetValue)])]
    public void Setup() => _dictionary = new Dictionary<int, int>(Input);

    [Benchmark(Baseline = true)]
    public void TryGetValue()
    {
        if (_dictionary.TryGetValue(5, out int value))
        {
            _dictionary[5] = value + 1;
        }
        else
        {
            _dictionary[5] = 1;
        }
    }

    [Benchmark]
    public void CollectionsMarshalTryGetValue()
    {
        CollectionsMarshal.GetValueRefOrAddDefault(_dictionary, 5, out _) += 1;
    }
}