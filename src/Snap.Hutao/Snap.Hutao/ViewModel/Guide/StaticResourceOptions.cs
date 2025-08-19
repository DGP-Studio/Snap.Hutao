// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Web.Hutao;
using System.Collections.Immutable;
using System.Globalization;

namespace Snap.Hutao.ViewModel.Guide;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class StaticResourceOptions : ObservableObject
{
    public ImmutableArray<NameValue<StaticResourceQuality>> ImageQualities { get; } = ImmutableCollectionsNameValue.FromEnum<StaticResourceQuality>(static q => q.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture) ?? string.Empty);

    public NameValue<StaticResourceQuality>? ImageQuality
    {
        get => field ??= ImageQualities.First(q => q.Value == UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageQuality, StaticResourceQuality.Original));
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.StaticResourceImageQuality, value.Value);
                UpdateSizeInformationText();
            }
        }
    }

    public ImmutableArray<NameValue<StaticResourceArchive>> ImageArchives { get; } = ImmutableCollectionsNameValue.FromEnum<StaticResourceArchive>(static a => a.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture) ?? string.Empty);

    public NameValue<StaticResourceArchive>? ImageArchive
    {
        get => field ??= ImageArchives.First(a => a.Value == UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageArchive, StaticResourceArchive.Full));
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.StaticResourceImageArchive, value.Value);
                UpdateSizeInformationText();
            }
        }
    }

    public StaticResourceSizeInformation? SizeInformation
    {
        get;
        set
        {
            field = value;
            UpdateSizeInformationText();
        }
    }

    [ObservableProperty]
    public partial string? SizeInformationText { get; set; }

    private void UpdateSizeInformationText()
    {
        if (SizeInformation is not null)
        {
            long result = (ImageQuality?.Value, ImageArchive?.Value) switch
            {
                (StaticResourceQuality.Original, StaticResourceArchive.Full) => SizeInformation.OriginalFull,
                (StaticResourceQuality.Original, StaticResourceArchive.Minimum) => SizeInformation.OriginalMinimum,
                (StaticResourceQuality.High, StaticResourceArchive.Full) => SizeInformation.HighFull,
                (StaticResourceQuality.High, StaticResourceArchive.Minimum) => SizeInformation.HighMinimum,
                _ => 0,
            };

            SizeInformationText = SH.FormatViewGuideStaticResourceDownloadSize(Converters.ToFileSizeString(result));
        }
    }
}