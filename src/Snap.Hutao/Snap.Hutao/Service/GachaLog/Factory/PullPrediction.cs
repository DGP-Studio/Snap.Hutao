// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Response;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 抽数预计
/// </summary>
internal sealed class PullPrediction
{
    private readonly TypedWishSummary typedWishSummary;
    private readonly TypedWishSummaryBuilderContext context;

    public PullPrediction(TypedWishSummary typedWishSummary, in TypedWishSummaryBuilderContext context)
    {
        this.typedWishSummary = typedWishSummary;
        this.context = context;
    }

    public async ValueTask PredictAsync(AsyncBarrier barrier)
    {
        await context.TaskContext.SwitchToBackgroundAsync();
        Response<GachaDistribution> response = await context.GetGachaDistributionAsync().ConfigureAwait(false);

        if (response is { ReturnCode: 0, Data: GachaDistribution data })
        {
            PredictResult result = PredictCore(data.Distribution, typedWishSummary);
            await barrier.SignalAndWaitAsync().ConfigureAwait(false);

            await context.TaskContext.SwitchToMainThreadAsync();
            typedWishSummary.ProbabilityOfNextPullIsOrange = result.ProbabilityOfNextPullIsOrange;
            typedWishSummary.ProbabilityOfPredictedPullLeftToOrange = result.ProbabilityOfPredictedPullLeftToOrange;
            typedWishSummary.PredictedPullLeftToOrange = result.PredictedPullLeftToOrange;
            typedWishSummary.IsPredictPullAvailable = true;
        }
    }

    private static PredictResult PredictCore(List<PullCount> distribution, TypedWishSummary typedWishSummary)
    {
        // 0 - 89/79
        ReadOnlySpan<double> baseFunction = ToProbabilityFunction(distribution, typedWishSummary.GuaranteeOrangeThreshold);

        // n - 89/79
        ReadOnlySpan<double> function = PartitionFunction(baseFunction[typedWishSummary.LastOrangePull..]);
        double nextProbability = function[0];
        int maxIndex = function.IndexOfMax();
        int leftToOrangePulls = maxIndex + 1;
        double leftToOrangeProbability = function[maxIndex];

        return new(nextProbability, leftToOrangeProbability, leftToOrangePulls);
    }

    /// <summary>
    /// 转换到概率函数
    /// </summary>
    /// <param name="distribution">分布</param>
    /// <returns>概率函数</returns>
    private static ReadOnlySpan<double> ToProbabilityFunction(List<PullCount> distribution, int threshold)
    {
        Span<double> results = new double[threshold];

        double totalCount = distribution.Sum(x => x.Count);
        foreach (ref readonly PullCount pullCount in CollectionsMarshal.AsSpan(distribution))
        {
            // Zero start index
            results[pullCount.Pull - 1] = pullCount.Count / totalCount;
        }

        return results;
    }

    private static ReadOnlySpan<double> PartitionFunction(in ReadOnlySpan<double> function)
    {
        double sum = Sum(function);
        Span<double> results = new double[function.Length];

        for (int i = 0; i < function.Length; i++)
        {
            results[i] = function[i] / sum;
        }

        return results;
    }

    private static double Sum(in ReadOnlySpan<double> function)
    {
        double sum = 0;
        foreach (ref readonly double probability in function)
        {
            sum += probability;
        }

        return sum;
    }

    private readonly struct PredictResult
    {
        public readonly double ProbabilityOfNextPullIsOrange;
        public readonly double ProbabilityOfPredictedPullLeftToOrange;
        public readonly int PredictedPullLeftToOrange;

        public PredictResult(double probabilityOfNextPullIsOrange, double probabilityOfPredictedPullLeftToOrange, int predictedPullLeftToOrange)
        {
            ProbabilityOfNextPullIsOrange = probabilityOfNextPullIsOrange;
            ProbabilityOfPredictedPullLeftToOrange = probabilityOfPredictedPullLeftToOrange;
            PredictedPullLeftToOrange = predictedPullLeftToOrange;
        }
    }
}