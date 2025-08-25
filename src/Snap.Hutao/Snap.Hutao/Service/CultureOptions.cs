// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;
using System.Globalization;

namespace Snap.Hutao.Service;

[ConstructorGenerated(CallBaseConstructor = true)]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class CultureOptions : DbStoreOptions
{
    private DayOfWeek? firstDayOfWeek;

    public ImmutableArray<NameCultureInfoValue> Cultures { get => SupportedCultures.Values; }

    [field: MaybeNull]
    public List<NameValue<DayOfWeek>> DayOfWeeks { get => field ??= Enum.GetValues<DayOfWeek>().Select(v => new NameValue<DayOfWeek>(CurrentCulture.DateTimeFormat.GetDayName(v), v)).ToList(); }

    [field: AllowNull]
    public CultureInfo CurrentCulture
    {
        get => GetOption(ref field, SettingEntry.Culture, CultureInfo.GetCultureInfo, CultureInfo.CurrentCulture);
        set => SetOption(ref field, SettingEntry.Culture, value, static v => v.Name);
    }

    public CultureInfo SystemCulture { get; set; } = default!;

    [field: MaybeNull]
    public string LocaleName { get => field ??= CultureOptionsExtension.GetLocaleName(CurrentCulture); }

    [field: AllowNull]
    [field: MaybeNull]
    public string LanguageCode
    {
        get
        {
            if (field is null && !LocaleNames.TryGetLanguageCodeFromLocaleName(LocaleName, out field))
            {
                throw new KeyNotFoundException($"Invalid localeName: '{LocaleName}'");
            }

            return field;
        }
    }

    public DayOfWeek FirstDayOfWeek
    {
        get => GetOption(ref firstDayOfWeek, SettingEntry.FirstDayOfWeek, Enum.Parse<DayOfWeek>, CurrentCulture.DateTimeFormat.FirstDayOfWeek);
        set => SetOption(ref firstDayOfWeek, SettingEntry.FirstDayOfWeek, value, EnumToStringOrEmpty);
    }
}