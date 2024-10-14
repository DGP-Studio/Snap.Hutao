// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Linq;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service;
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

        CalendarMetadataContext metadataContext = await metadataService.GetContextAsync<CalendarMetadataContext>().ConfigureAwait(false);
        ILookup<MonthAndDay, Avatar> avatars = metadataContext.Avatars.ToLookup(a => new MonthAndDay(a.FetterInfo.BirthMonth, a.FetterInfo.BirthDay));
        ILookup<MaterialId, Item> materials = GetMaterialItemLookup(metadataContext);

        CalendarMetadataContext2 context2 = new()
        {
            MetadataContext = metadataContext,
            AvatarBirthdays = avatars,
            MaterialItems = materials,
        };

        ImmutableArray<CalendarMaterial> materials1 = [.. EnumerateMaterials(MaterialIds.MondayThursdayHighestRankItems, context2).OrderBy(m => (uint)m.Inner.Id)];
        ImmutableArray<CalendarMaterial> materials2 = [.. EnumerateMaterials(MaterialIds.TuesdayFridayHighestRankItems, context2).OrderBy(m => (uint)m.Inner.Id)];
        ImmutableArray<CalendarMaterial> materials3 = [.. EnumerateMaterials(MaterialIds.WednesdaySaturdayHighestRankItems, context2).OrderBy(m => (uint)m.Inner.Id)];
        Dictionary<DayOfWeek, ImmutableArray<CalendarMaterial>> dailyMaterials = new()
        {
            [DayOfWeek.Monday] = materials1,
            [DayOfWeek.Tuesday] = materials2,
            [DayOfWeek.Wednesday] = materials3,
            [DayOfWeek.Thursday] = materials1,
            [DayOfWeek.Friday] = materials2,
            [DayOfWeek.Saturday] = materials3,
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
            BirthDayAvatars = [.. context.AvatarBirthdays[new MonthAndDay((uint)date.Month, (uint)date.Day)].Select(a => a.ToItem())],
            Materials = dailyMaterials.GetValueOrDefault(date.DayOfWeek, []),
        };
    }

    private static ILookup<MaterialId, Item> GetMaterialItemLookup(CalendarMetadataContext metadataContext)
    {
        Dictionary<MaterialId, List<Item>> results = [];

        foreach (ref readonly Avatar avatar in metadataContext.Avatars.AsSpan())
        {
            ref List<Item>? group = ref CollectionsMarshal.GetValueRefOrAddDefault(results, avatar.CultivationItems[4], out bool exist);
            if (!exist)
            {
                group = [];
            }

            ArgumentNullException.ThrowIfNull(group);
            group.Add(avatar.ToItem());
        }

        foreach (ref readonly Weapon weapon in metadataContext.Weapons.AsSpan())
        {
            ref List<Item>? group = ref CollectionsMarshal.GetValueRefOrAddDefault(results, weapon.CultivationItems[0], out bool exist);
            if (!exist)
            {
                group = [];
            }

            ArgumentNullException.ThrowIfNull(group);
            group.Add(weapon.ToItem());
        }

        return results.ToLookup();
    }

    private static IEnumerable<CalendarMaterial> EnumerateMaterials(IReadOnlySet<MaterialId> ids, CalendarMetadataContext2 context)
    {
        foreach (MaterialId id in ids)
        {
            if (!context.MetadataContext.IdMaterialMap.TryGetValue(id, out Material? material))
            {
                continue;
            }

            yield return new()
            {
                Inner = material,
                Items = [.. context.MaterialItems[id].OrderByDescending(i => i.Quality)],
            };
        }
    }
}