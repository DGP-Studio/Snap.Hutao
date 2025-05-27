// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Service;
using Snap.Hutao.Web.Hoyolab;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingHomeViewModel : Abstraction.ViewModel
{
    partial void PostConstruct(IServiceProvider serviceProvider)
    {
        List<SettingHomeCardViewModel> viewModels =
        [
            new(SH.ViewPageSettingHomeCardItemLaunchGameHeader, SettingKeys.IsHomeCardLaunchGamePresented, SettingKeys.HomeCardLaunchGameOrder),
            new(SH.ViewPageSettingHomeCardItemgachaStatisticsHeader, SettingKeys.IsHomeCardGachaStatisticsPresented, SettingKeys.HomeCardGachaStatisticsOrder),
            new(SH.ViewPageSettingHomeCardItemAchievementHeader, SettingKeys.IsHomeCardAchievementPresented, SettingKeys.HomeCardAchievementOrder),
            new(SH.ViewPageSettingHomeCardItemDailyNoteHeader, SettingKeys.IsHomeCardDailyNotePresented, SettingKeys.HomeCardDailyNoteOrder),
            new(SH.ViewPageSettingHomeCardItemCalendarHeader, SettingKeys.IsHomeCardCalendarPresented, SettingKeys.HomeCardCalendarOrder),
            new(SH.ViewPageSettingHomeCardItemSignInHeader, SettingKeys.IsHomeCardSignInPresented, SettingKeys.HomeCardSignInOrder),
        ];

        viewModels.SortBy(v => v.Order);

        HomeCards = new SettingHomeCardObservableCollection(viewModels);
    }

    public HomeCardOptions HomeCardOptions { get; } = new();

    public partial AppOptions AppOptions { get; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public NameValue<Region>? SelectedRegion
    {
        get => field ??= Selection.Initialize(AppOptions.LazyRegions, AppOptions.Region);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.Region = value.Value;
            }
        }
    }

    public NameValue<TimeSpan>? SelectedCalendarServerTimeZoneOffset
    {
        get => field ??= Selection.Initialize(AppOptions.LazyCalendarServerTimeZoneOffsets, AppOptions.CalendarServerTimeZoneOffset);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.CalendarServerTimeZoneOffset = value.Value;
            }
        }
    }

    public ObservableCollection<SettingHomeCardViewModel>? HomeCards { get; private set; }
}