// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Globalization;

namespace Snap.Hutao.Service;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Singleton)]
internal sealed partial class CultureOptions : DbStoreOptions
{
    private CultureInfo? currentCulture;
    private string? localeName;
    private string? languageCode;

    public List<NameValue<CultureInfo>> Cultures { get; } = SupportedCultures.Get();

    public CultureInfo CurrentCulture
    {
        get => GetOption(ref currentCulture, SettingEntry.Culture, CultureInfo.GetCultureInfo, CultureInfo.CurrentCulture);
        set => SetOption(ref currentCulture, SettingEntry.Culture, value, value => value.Name);
    }

    public CultureInfo SystemCulture { get; set; } = default!;

    public string LocaleName { get => localeName ??= CultureOptionsExtension.GetLocaleName(CurrentCulture); }

    public string LanguageCode
    {
        get
        {
            if (languageCode is null && !LocaleNames.TryGetLanguageCodeFromLocaleName(LocaleName, out languageCode))
            {
                throw new KeyNotFoundException($"Invalid localeName: '{LocaleName}'");
            }

            return languageCode;
        }
    }
}