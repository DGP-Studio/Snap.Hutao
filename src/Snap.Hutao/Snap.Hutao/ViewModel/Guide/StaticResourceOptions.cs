// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Model;

namespace Snap.Hutao.ViewModel.Guide;

[Injection(InjectAs.Singleton)]
internal sealed class StaticResourceOptions
{
    private readonly List<NameValue<StaticResourceQuality>> imageQualities = CollectionsNameValue.FromEnum<StaticResourceQuality>(q => q.GetLocalizedDescription());

    private NameValue<StaticResourceQuality>? imageQuality;

    public StaticResourceOptions()
    {
        ImageQuality = ImageQualities.First(q => q.Value == UnsafeLocalSetting.Get(SettingKeys.StaticResourceImageQuality, StaticResourceQuality.Raw));
    }

    public List<NameValue<StaticResourceQuality>> ImageQualities { get => imageQualities; }

    public NameValue<StaticResourceQuality>? ImageQuality
    {
        get => imageQuality;
        set
        {
            if (value is not null)
            {
                imageQuality = value;
                UnsafeLocalSetting.Set(SettingKeys.StaticResourceImageQuality, value.Value);
            }
        }
    }

    public bool UseTrimmedArchive
    {
        get => LocalSetting.Get(SettingKeys.StaticResourceUseTrimmedArchive, false);
        set => LocalSetting.Set(SettingKeys.StaticResourceUseTrimmedArchive, value);
    }
}