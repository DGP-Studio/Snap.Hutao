// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal sealed class PullPrediction
{
    private readonly TypedWishSummary typedWishSummary;
    private readonly TypedWishSummaryBuilderContext context;

    public PullPrediction(TypedWishSummary typedWishSummary, in TypedWishSummaryBuilderContext context)
    {
        this.typedWishSummary = typedWishSummary;
        this.context = context;
    }

    [SuppressMessage("", "SH003")]
    public async Task PredictAsync(AsyncBarrier barrier)
    {
        await context.TaskContext.SwitchToBackgroundAsync();
        Response<GachaDistribution>? response = await context.GetGachaDistributionAsync().ConfigureAwait(false);

        if (response is { ReturnCode: 0, Data: { } data })
        {
            PredictResult result = PrivatePredict(data.Distribution, typedWishSummary);
            await barrier.SignalAndWaitAsync().ConfigureAwait(false);

            await context.TaskContext.SwitchToMainThreadAsync();
            typedWishSummary.ProbabilityOfNextPullIsOrange = result.ProbabilityOfNextPullIsOrange;
            typedWishSummary.ProbabilityOfPredictedPullLeftToOrange = result.ProbabilityOfPredictedPullLeftToOrange;
            typedWishSummary.PredictedPullLeftToOrange = result.PredictedPullLeftToOrange;
            typedWishSummary.IsPredictPullAvailable = true;
        }
        else
        {
            await barrier.SignalAndWaitAsync().ConfigureAwait(false);
        }
    }

    private static PredictResult PrivatePredict(ImmutableArray<PullCount> distribution, TypedWishSummary typedWishSummary)
    {
        // 0 - 89/79
        ImmutableArray<double> baseFunction = ToProbabilityFunction(distribution, typedWishSummary.GuaranteeOrangeThreshold);

        // n - 89/79
        int lastOrangePull = Math.Clamp(typedWishSummary.LastOrangePull, 0, baseFunction.Length - 1);
        ImmutableArray<double> function = GetConditionalProbabilityFunction(baseFunction.AsSpan()[lastOrangePull..]);
        ReadOnlySpan<double> functionSpan = function.AsSpan();

        double nextProbability = functionSpan[0];
        int maxIndex = functionSpan.IndexOfMax();
        int leftToOrangePulls = maxIndex + 1;
        double leftToOrangeProbability = functionSpan[maxIndex];

        return new(nextProbability, leftToOrangeProbability, leftToOrangePulls);
    }

    private static ImmutableArray<double> ToProbabilityFunction(ImmutableArray<PullCount> distribution, int threshold)
    {
        double totalCount = distribution.Sum(x => x.Count);
        double[] results = new double[threshold];
        Span<double> span = results;

        foreach (ref readonly PullCount pullCount in distribution.AsSpan())
        {
            // Zero start index
            span[pullCount.Pull - 1] = pullCount.Count / totalCount;
        }

        return ImmutableCollectionsMarshal.AsImmutableArray(results);
    }

    private static ImmutableArray<double> GetConditionalProbabilityFunction(in ReadOnlySpan<double> function)
    {
        double totalCount = Sum(function);
        double[] results = new double[function.Length];
        Span<double> span = results;

        for (int i = 0; i < function.Length; i++)
        {
            span[i] = function[i] / totalCount;
        }

        return ImmutableCollectionsMarshal.AsImmutableArray(results);
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