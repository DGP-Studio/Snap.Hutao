using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Snap.Hutao.Benchmark;

public class Program
{
    static void Main(string[] args)
    {
        // BenchmarkRunner.Run<LevelExperienceBenchmark>();
        BenchmarkRunner.Run<DictionaryBenchmark>();
    }
}