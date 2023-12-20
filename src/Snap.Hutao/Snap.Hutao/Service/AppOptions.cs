// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using Snap.Hutao.Core.Windowing;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab;
using System.Globalization;
using System.IO;

namespace Snap.Hutao.Service;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Singleton)]
internal sealed partial class AppOptions : DbStoreOptions
{
    private string? powerShellPath;
    private bool? isEmptyHistoryWishVisible;
    private BackdropType? backdropType;
    private CultureInfo? currentCulture;
    private Region? region;
    private string? geetestCustomCompositeUrl;

    public string PowerShellPath
    {
        get
        {
            return GetOption(ref powerShellPath, SettingEntry.PowerShellPath, GetDefaultPowerShellLocationOrEmpty);

            static string GetDefaultPowerShellLocationOrEmpty()
            {
                string? paths = Environment.GetEnvironmentVariable("Path");
                if (!string.IsNullOrEmpty(paths))
                {
                    foreach (StringSegment path in new StringTokenizer(paths, [';']))
                    {
                        if (path is { HasValue: true, Length: > 0 })
                        {
                            if (path.Value.Contains("WindowsPowerShell", StringComparison.OrdinalIgnoreCase))
                            {
                                return Path.Combine(path.Value, "powershell.exe");
                            }
                        }
                    }
                }

                return string.Empty;
            }
        }
        set => SetOption(ref powerShellPath, SettingEntry.PowerShellPath, value);
    }

    public bool IsEmptyHistoryWishVisible
    {
        get => GetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible);
        set => SetOption(ref isEmptyHistoryWishVisible, SettingEntry.IsEmptyHistoryWishVisible, value);
    }

    public List<NameValue<BackdropType>> BackdropTypes { get; } = CollectionsNameValue.FromEnum<BackdropType>();

    public BackdropType BackdropType
    {
        get => GetOption(ref backdropType, SettingEntry.SystemBackdropType, v => Enum.Parse<BackdropType>(v), BackdropType.Mica).Value;
        set => SetOption(ref backdropType, SettingEntry.SystemBackdropType, value, value => value.ToStringOrEmpty());
    }

    public List<NameValue<CultureInfo>> Cultures { get; } = SupportedCultures.Get();

    public CultureInfo CurrentCulture
    {
        get => GetOption(ref currentCulture, SettingEntry.Culture, CultureInfo.GetCultureInfo, CultureInfo.CurrentCulture);
        set => SetOption(ref currentCulture, SettingEntry.Culture, value, value => value.Name);
    }

    public List<NameValue<Region>> Regions { get; } = KnownRegions.Get();

    public Region Region
    {
        get => GetOption(ref region, SettingEntry.AnnouncementRegion, v => Region.FromRegionString(v), Region.CNGF01).Value;
        set => SetOption(ref region, SettingEntry.AnnouncementRegion, value, value => value.ToStringOrEmpty());
    }

    public string GeetestCustomCompositeUrl
    {
        get => GetOption(ref geetestCustomCompositeUrl, SettingEntry.GeetestCustomCompositeUrl);
        set => SetOption(ref geetestCustomCompositeUrl, SettingEntry.GeetestCustomCompositeUrl, value);
    }

    internal CultureInfo PreviousCulture { get; set; } = default!;
}