// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.Linq;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.Calendar;

[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class CalendarViewModel : Abstraction.ViewModelSlim
{
    private readonly ICultivationService cultivationService;
    private readonly ILogger<CalendarViewModel> logger;
    private readonly IMetadataService metadataService;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;

    private AdvancedCollectionView<CalendarDay>? weekDays;

    public AdvancedCollectionView<CalendarDay>? WeekDays { get => weekDays; set => SetProperty(ref weekDays, value); }

    protected override async Task LoadAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return;
        }

        AdvancedCollectionView<CalendarDay> weekDays;
        using (ValueStopwatch.MeasureExecution(logger, nameof(CreateWeekDays)))
        {
            weekDays = await CreateWeekDays().ConfigureAwait(false);
        }

        await taskContext.SwitchToMainThreadAsync();

        WeekDays = weekDays;
        WeekDays.MoveCurrentTo(WeekDays.SourceCollection.SingleOrDefault(d => d.Date == DateTimeOffset.Now.Date));
        IsInitialized = true;
    }

    private static CalendarDay CreateCalendarDay(DateTimeOffset date, CalendarMetadataContext2 context, IReadOnlyDictionary<DayOfWeek, ImmutableArray<CalendarMaterial>> dailyMaterials)
    {
        DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;

        return new()
        {
            Date = date,
            DayInMonth = date.Day,
            DayName = dtfi.GetAbbreviatedDayName(date.DayOfWeek),
            BirthDayAvatars = [.. context.AvatarBirthdays[new MonthAndDay((uint)date.Month, (uint)date.Day)].Select(a => a.ToItem<Item>())],
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

        return results.ToLookup();
    }

    private static IEnumerable<CalendarMaterial> EnumerateMaterials(IReadOnlySet<RotationalMaterialIdEntry> entries, CalendarMetadataContext2 context)
    {
        foreach (RotationalMaterialIdEntry entry in entries)
        {
            if (!context.MetadataContext.IdMaterialMap.TryGetValue(entry.Highest, out Material? material))
            {
                continue;
            }

            yield return new()
            {
                Inner = material,
                Items = [.. context.MaterialItems[entry.Highest].OrderByDescending(i => i.Quality)],
                Highlight = false,
            };
        }
    }

    private async ValueTask<AdvancedCollectionView<CalendarDay>> CreateWeekDays()
    {
        CalendarMetadataContext metadataContext = await metadataService.GetContextAsync<CalendarMetadataContext>().ConfigureAwait(false);
        ILookup<MonthAndDay, Avatar> avatarBirthdays = metadataContext.Avatars.ToLookup(MonthAndDay.Create);
        ILookup<MaterialId, CalendarItem> materialItems = GetMaterialItemLookup(metadataContext);

        CalendarMetadataContext2 context2 = new()
        {
            MetadataContext = metadataContext,
            AvatarBirthdays = avatarBirthdays,
            MaterialItems = materialItems,
            CultivateEntryViews = await cultivationService.GetCultivateEntryCollectionForCurrentProjectAsync(metadataContext).ConfigureAwait(false),
        };

        ImmutableArray<CalendarMaterial> materials14 = [.. EnumerateMaterials(MaterialIds.MondayThursdayEntries, context2).OrderBy(m => (uint)m.Inner.Id)];
        ImmutableArray<CalendarMaterial> materials25 = [.. EnumerateMaterials(MaterialIds.TuesdayFridayEntries, context2).OrderBy(m => (uint)m.Inner.Id)];
        ImmutableArray<CalendarMaterial> materials36 = [.. EnumerateMaterials(MaterialIds.WednesdaySaturdayEntries, context2).OrderBy(m => (uint)m.Inner.Id)];
        Dictionary<DayOfWeek, ImmutableArray<CalendarMaterial>> dailyMaterials = new()
        {
            [DayOfWeek.Monday] = materials14,
            [DayOfWeek.Tuesday] = materials25,
            [DayOfWeek.Wednesday] = materials36,
            [DayOfWeek.Thursday] = materials14,
            [DayOfWeek.Friday] = materials25,
            [DayOfWeek.Saturday] = materials36,
        };

        DateTimeOffset today = DateTimeOffset.Now.Date;
        DayOfWeek firstDayOfWeek = cultureOptions.FirstDayOfWeek;
        DateTimeOffset nearestStartOfWeek = today.AddDays((int)firstDayOfWeek - (int)today.DayOfWeek);
        if (nearestStartOfWeek > today)
        {
            nearestStartOfWeek = nearestStartOfWeek.AddDays(-7);
        }

        AdvancedCollectionView<CalendarDay> weekDays = Enumerable
            .Range(0, 7)
            .Select(i => CreateCalendarDay(nearestStartOfWeek.AddDays(i), context2, dailyMaterials))
            .ToAdvancedCollectionView();
        return weekDays;
    }
}