// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.Data.Converter.Specialized;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;

namespace Snap.Hutao.ViewModel.HardChallenge;

internal sealed partial class DataEntryView : IPropertyValuesProvider
{
    private DataEntryView(bool singlePlayer, HardChallengeDataEntry dataEntry, HardChallengeMetadataContext context)
    {
        Name = singlePlayer
            ? SH.ViewModelHardChalllengeDataEntrySinglePlayerName
            : SH.ViewModelHardChalllengeDataEntryMultiPlayerName;
        Debug.Assert(dataEntry.HasData);
        DifficultyIcon = HardChallengeDifficultyIconConverter.Convert(dataEntry.Best.Icon.Split(',').Last());
        Difficulty = dataEntry.Best.Difficulty.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture);
        FormattedSeconds = SH.FormatViewModelHardChallengeSeconds(dataEntry.Best.Seconds);

        Challenges = dataEntry.Challenges.SelectAsArray(ChallengeView.Create, context);
    }

    public string Name { get; }

    public Uri DifficultyIcon { get; }

    public string? Difficulty { get; }

    public string FormattedSeconds { get; }

    public ImmutableArray<ChallengeView> Challenges { get; }

    public static DataEntryView? Create(bool singlePlayer, HardChallengeDataEntry dataEntry, HardChallengeMetadataContext context)
    {
        if (!dataEntry.HasData)
        {
            return default;
        }

        return new(singlePlayer, dataEntry, context);
    }
}