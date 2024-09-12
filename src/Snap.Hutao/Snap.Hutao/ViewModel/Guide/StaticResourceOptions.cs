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

    private NameValue<StaticResourceQuality>? imageQuality;
    private NameValue<StaticResourceArchive>? imageArchive;
    private string? sizeInformationText;

    private StaticResourceSizeInformation? sizeInformation;

    public List<NameValue<StaticResourceQuality>> ImageQualities { get => imageQualities; }

    public NameValue<StaticResourceQuality>? ImageQuality
    {
        get => imageQuality ??= ImageQualities.First(q => q.Value == UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageQuality, StaticResourceQuality.Raw));
        set
        {
            if (SetProperty(ref imageQuality, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.StaticResourceImageQuality, value.Value);
                UpdateSizeInformationText();
            }
        }
    }

    public List<NameValue<StaticResourceArchive>> ImageArchives { get => imageArchives; }

    public NameValue<StaticResourceArchive>? ImageArchive
    {
        get => imageArchive ??= ImageArchives.First(a => a.Value == UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageArchive, StaticResourceArchive.Full));
        set
        {
            if (SetProperty(ref imageArchive, value) && value is not null)
            {
                UnsafeLocalSetting.Set(SettingKeys.StaticResourceImageArchive, value.Value);
                UpdateSizeInformationText();
            }
        }
    }

    public StaticResourceSizeInformation? SizeInformation
    {
        get => sizeInformation;
        set
        {
            sizeInformation = value;
            UpdateSizeInformationText();
        }
    }

    public string? SizeInformationText { get => sizeInformationText; set => SetProperty(ref sizeInformationText, value); }

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