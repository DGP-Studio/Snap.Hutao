// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.ViewModel.Calendar;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient)]
internal sealed partial class CalendarViewModel : Abstraction.ViewModelSlim
{
    private List<CalendarDay>? weekDays;

    public List<CalendarDay>? WeekDays { get => weekDays; set => SetProperty(ref weekDays, value); }

    protected override Task LoadAsync()
    {
        DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        DateTimeOffset today = DateTimeOffset.Now.Date;
        DateTimeOffset startOfWeek = today.AddDays((int)firstDayOfWeek - (int)today.DayOfWeek);
        List<CalendarDay> weekDays =
        [
            CreateCalendarDay(startOfWeek),
            CreateCalendarDay(startOfWeek.AddDays(1)),
            CreateCalendarDay(startOfWeek.AddDays(2)),
            CreateCalendarDay(startOfWeek.AddDays(3)),
            CreateCalendarDay(startOfWeek.AddDays(4)),
            CreateCalendarDay(startOfWeek.AddDays(5)),
            CreateCalendarDay(startOfWeek.AddDays(6))
        ];

        WeekDays = weekDays;
        return Task.CompletedTask;
    }

    private static CalendarDay CreateCalendarDay(DateTimeOffset date)
    {
        return new CalendarDay
        {
            Date = date,
            DayInMonth = date.Day,
            IsToday = date.Date == DateTimeOffset.Now.Date,
        };
    }
}