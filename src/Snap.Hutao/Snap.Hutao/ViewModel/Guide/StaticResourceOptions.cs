// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;
using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.ViewModel.Guide;

[Injection(InjectAs.Singleton)]
internal sealed class StaticResourceOptions : ObservableObject
{
    private readonly List<NameValue<StaticResourceQuality>> imageQualities = CollectionsNameValue.FromEnum<StaticResourceQuality>(q => q.GetLocalizedDescription());

    private NameValue<StaticResourceQuality>? imageQuality;
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

    public bool UseTrimmedArchive
    {
        get => LocalSetting.Get(SettingKeys.StaticResourceUseTrimmedArchive, false);
        set
        {
            LocalSetting.Set(SettingKeys.StaticResourceUseTrimmedArchive, value);
            UpdateSizeInformationText();
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
            long result = (ImageQuality?.Value, UseTrimmedArchive) switch
            {
                (StaticResourceQuality.Raw, false) => SizeInformation.RawFull,
                (StaticResourceQuality.Raw, true) => SizeInformation.RawMinimum,
                (StaticResourceQuality.High, false) => SizeInformation.HighFull,
                (StaticResourceQuality.High, true) => SizeInformation.HighMinimum,
                _ => 0,
            };

            SizeInformationText = SH.FormatViewGuideStaticResourceDownloadSize(Converters.ToFileSizeString(result));
        }
    }
}