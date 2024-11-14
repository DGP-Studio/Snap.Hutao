// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.ViewModel.Guide;

[Injection(InjectAs.Singleton)]
internal sealed partial class StaticResourceOptions : ObservableObject
{
    private readonly List<NameValue<StaticResourceQuality>> imageQualities = CollectionsNameValue.FromEnum<StaticResourceQuality>(q => q.GetLocalizedDescription());
    private readonly List<NameValue<StaticResourceArchive>> imageArchives = CollectionsNameValue.FromEnum<StaticResourceArchive>(a => a.GetLocalizedDescription());

    public List<NameValue<StaticResourceQuality>> ImageQualities { get => imageQualities; }

    public NameValue<StaticResourceQuality>? ImageQuality
    {
        get => field ??= ImageQualities.First(q => q.Value == UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageQuality, StaticResourceQuality.Raw));
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.StaticResourceImageQuality, value.Value);
                UpdateSizeInformationText();
            }
        }
    }

    public List<NameValue<StaticResourceArchive>> ImageArchives { get => imageArchives; }

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

    public string? SizeInformationText { get; set => SetProperty(ref field, value); }

    private void UpdateSizeInformationText()
    {
        if (SizeInformation is not null)
        {
            long result = (ImageQuality?.Value, ImageArchive?.Value) switch
            {
                (StaticResourceQuality.Raw, StaticResourceArchive.Full) => SizeInformation.RawFull,
                (StaticResourceQuality.Raw, StaticResourceArchive.Minimum) => SizeInformation.RawMinimum,
                (StaticResourceQuality.High, StaticResourceArchive.Full) => SizeInformation.HighFull,
                (StaticResourceQuality.High, StaticResourceArchive.Minimum) => SizeInformation.HighMinimum,
                _ => 0,
            };

            SizeInformationText = SH.FormatViewGuideStaticResourceDownloadSize(Converters.ToFileSizeString(result));
        }
    }
}