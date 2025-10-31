// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Property;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;
using System.Globalization;

namespace Snap.Hutao.Service;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class CultureOptions : DbStoreOptions
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial CultureOptions(IServiceProvider serviceProvider);

    public static ImmutableArray<NameCultureInfoValue> Cultures { get; } = SupportedCultures.GetValues();

    public ImmutableArray<NameValue<DayOfWeek>> DayOfWeeks { get => !field.IsDefaultOrEmpty ? field : field = ImmutableCollectionsNameValue.FromEnum<DayOfWeek>(CurrentCulture.Value.DateTimeFormat.GetDayName); }

    [field: MaybeNull]
    public IObservableProperty<CultureInfo> CurrentCulture { get => field ??= CreatePropertyForClassUsingCustom(SettingEntry.Culture, CultureInfo.CurrentCulture, CultureInfo.GetCultureInfo, static v => v.Name); }

    public CultureInfo SystemCulture { get; set; } = default!;

    public string LocaleName { get => LocaleNames.GetLocaleName(CurrentCulture.Value); }

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

    [field: MaybeNull]
    public IObservableProperty<DayOfWeek> FirstDayOfWeek { get => field ??= CreateProperty(SettingEntry.FirstDayOfWeek, CurrentCulture.Value.DateTimeFormat.FirstDayOfWeek); }
}