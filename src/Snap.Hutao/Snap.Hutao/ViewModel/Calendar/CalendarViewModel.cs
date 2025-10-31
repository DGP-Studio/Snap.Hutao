// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.Calendar;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class CalendarViewModel : Abstraction.ViewModelSlim<CultivationPage>
{
    private readonly ICultivationService cultivationService;
    private readonly ILogger<CalendarViewModel> logger;
    private readonly IMetadataService metadataService;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial CalendarViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial IAdvancedCollectionView<CalendarDay>? WeekDays { get; set; }

    protected override async Task LoadAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return;
        }

        CultivateProject? cultivateProject = await cultivationService.GetCurrentProjectAsync().ConfigureAwait(false);
        TimeSpan serverTimeZoneOffset = cultivateProject?.ServerTimeZoneOffset ?? appOptions.CalendarServerTimeZoneOffset.Value;

        IAdvancedCollectionView<CalendarDay> weekDays;
        using (ValueStopwatch.MeasureExecution(logger, nameof(CreateWeekDays)))
        {
            weekDays = await CreateWeekDays(serverTimeZoneOffset).ConfigureAwait(false);
        }

        DateTime effectiveToday = DateTimeOffset.Now.ToOffset(serverTimeZoneOffset).Date;
        await taskContext.SwitchToMainThreadAsync();

        WeekDays = weekDays;
        WeekDays.MoveCurrentTo(WeekDays.Source.SingleOrDefault(d => d.Date == effectiveToday));
        IsInitialized = true;
    }

    private static CalendarDay CreateCalendarDay(DateTimeOffset date, CalendarMetadataContext2 context2, IReadOnlyDictionary<DayOfWeek, ImmutableArray<CalendarMaterial>> dailyMaterials)
    {
        DateTimeFormatInfo formatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

        return new()
        {
            Date = date,
            DayInMonth = date.Day,
            DayName = formatInfo.GetAbbreviatedDayName(date.DayOfWeek),
            BirthDayAvatars = [.. context2.AvatarBirthdays[new((uint)date.Month, (uint)date.Day)].Select(a => a.ToItem<Item>())],
            Materials = dailyMaterials.GetValueOrDefault(date.DayOfWeek, []),
        };
    }

    private static ILookup<MaterialId, CalendarItem> GetMaterialItemLookup(CalendarMetadataContext metadataContext)
    {
        Dictionary<MaterialId, List<CalendarItem>> results = [];

        foreach (ref readonly Avatar avatar in metadataContext.Avatars.AsSpan())
        {
            ref List<CalendarItem>? group = ref CollectionsMarshal.GetValueRefOrAddDefault(results, avatar.CultivationItems[4], out bool exist);
            if (!exist)
            {
                group = [];
            }

            ArgumentNullException.ThrowIfNull(group);
            group.Add(avatar.ToItem<CalendarItem>());
        }

        foreach (ref readonly Weapon weapon in metadataContext.Weapons.AsSpan())
        {
            ref List<CalendarItem>? group = ref CollectionsMarshal.GetValueRefOrAddDefault(results, weapon.CultivationItems[0], out bool exist);
            if (!exist)
            {
                group = [];
            }

            ArgumentNullException.ThrowIfNull(group);
            group.Add(weapon.ToItem<CalendarItem>());
        }

        return results
            .SelectMany(kvp => kvp.Value.Select(item => KeyValuePair.Create(kvp.Key, item)))
            .ToLookup(d => d.Key, d => d.Value);
    }

    private static IEnumerable<CalendarMaterial> EnumerateMaterials(IReadOnlySet<RotationalMaterialIdEntry> entries, CalendarMetadataContext2 context2)
    {
        foreach (RotationalMaterialIdEntry entry in entries)
        {
            if (!context2.MetadataContext.IdMaterialMap.TryGetValue(entry.Highest, out Material? material))
            {
                continue;
            }

            yield return new()
            {
                Inner = material,
                InnerEntry = entry,
                Items = [.. context2.MaterialItems[entry.Highest].OrderByDescending(i => i.Quality)],
                Highlight = false,
            };
        }
    }

    private static Void InitializeHighlightItems(ImmutableArray<CalendarMaterial> materials, CultivateEntryView view)
    {
        foreach (ref readonly CalendarMaterial material in materials.AsSpan())
        {
            if (material.InnerEntry.Set.IsSupersetOf(view.RotationalItemIds))
            {
                material.Highlight = true;
                foreach (ref readonly CalendarItem item in material.Items.AsSpan())
                {
                    if (view.Id == item.Id)
                    {
                        item.Highlight = true;
                        break;
                    }
                }

                break;
            }
        }

        return default;
    }

    private async ValueTask<IAdvancedCollectionView<CalendarDay>> CreateWeekDays(TimeSpan serverTimeZoneOffset)
    {
        CalendarMetadataContext metadataContext = await metadataService.GetContextAsync<CalendarMetadataContext>().ConfigureAwait(false);
        ILookup<MonthAndDay, Avatar> avatarBirthdays = metadataContext.Avatars.ToLookup(MonthAndDay.Create);
        ILookup<MaterialId, CalendarItem> materialItems = GetMaterialItemLookup(metadataContext);

        CalendarMetadataContext2 context2 = new()
        {
            MetadataContext = metadataContext,
            AvatarBirthdays = avatarBirthdays,
            MaterialItems = materialItems,
        };

        ImmutableArray<CalendarMaterial> materials14 = [.. EnumerateMaterials(MaterialIds.MondayThursdayEntries, context2).OrderBy(m => (uint)m.Inner.Id)];
        ImmutableArray<CalendarMaterial> materials25 = [.. EnumerateMaterials(MaterialIds.TuesdayFridayEntries, context2).OrderBy(m => (uint)m.Inner.Id)];
        ImmutableArray<CalendarMaterial> materials36 = [.. EnumerateMaterials(MaterialIds.WednesdaySaturdayEntries, context2).OrderBy(m => (uint)m.Inner.Id)];

        ObservableCollection<CultivateEntryView>? entries = await cultivationService.GetCultivateEntryCollectionForCurrentProjectAsync(metadataContext).ConfigureAwait(false);
        if (entries is not null)
        {
            foreach (CultivateEntryView view in entries)
            {
                _ = view.DaysOfWeek switch
                {
                    DaysOfWeek.MondayAndThursday => InitializeHighlightItems(materials14, view),
                    DaysOfWeek.TuesdayAndFriday => InitializeHighlightItems(materials25, view),
                    DaysOfWeek.WednesdayAndSaturday => InitializeHighlightItems(materials36, view),
                    _ => default,
                };
            }
        }

        Dictionary<DayOfWeek, ImmutableArray<CalendarMaterial>> dailyMaterials = new()
        {
            [DayOfWeek.Monday] = materials14,
            [DayOfWeek.Tuesday] = materials25,
            [DayOfWeek.Wednesday] = materials36,
            [DayOfWeek.Thursday] = materials14,
            [DayOfWeek.Friday] = materials25,
            [DayOfWeek.Saturday] = materials36,
        };

        DateTimeOffset effectiveToday = DateTimeOffset.Now.ToOffset(serverTimeZoneOffset).Date;
        DayOfWeek firstDayOfWeek = cultureOptions.FirstDayOfWeek.Value;
        DateTimeOffset nearestStartOfWeek = effectiveToday.AddDays((int)firstDayOfWeek - (int)effectiveToday.DayOfWeek);
        if (nearestStartOfWeek > effectiveToday)
        {
            nearestStartOfWeek = nearestStartOfWeek.AddDays(-7);
        }

        IAdvancedCollectionView<CalendarDay> weekDays = Enumerable
            .Range(0, 7)
            .Select(i => CreateCalendarDay(nearestStartOfWeek.AddDays(i), context2, dailyMaterials))
            .AsAdvancedCollectionView();
        return weekDays;
    }
}