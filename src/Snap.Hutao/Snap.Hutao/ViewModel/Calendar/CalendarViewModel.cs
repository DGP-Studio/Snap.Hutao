// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.Data;
using System.Collections.Immutable;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Calendar;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient)]
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

        DateTimeOffset today = DateTimeOffset.Now.Date;
        DayOfWeek firstDayOfWeek = cultureOptions.FirstDayOfWeek;
        DateTimeOffset startOfWeek = today.AddDays((int)firstDayOfWeek - (int)today.DayOfWeek);

        AdvancedCollectionView<CalendarDay> weekDays = Enumerable.Range(0, 7)
            .Select(i => CreateCalendarDay(startOfWeek.AddDays(i), avatars))
            .ToAdvancedCollectionView();

        await taskContext.SwitchToMainThreadAsync();

        WeekDays = weekDays;
        WeekDays.MoveCurrentTo(WeekDays.SourceCollection.SingleOrDefault(d => d.Date == DateTimeOffset.Now.Date));
    }

    private static CalendarDay CreateCalendarDay(DateTimeOffset date, ILookup<MonthAndDay, Avatar> avatars)
    {
        DateTimeFormatInfo dtfi = CultureInfo.CurrentCulture.DateTimeFormat;

        return new()
        {
            Date = date,
            DayInMonth = date.Day,
            DayName = dtfi.GetAbbreviatedDayName(date.DayOfWeek),
            BirthDayAvatars = avatars[new MonthAndDay((uint)date.Month, (uint)date.Day)].ToImmutableArray(),
        };
    }
}