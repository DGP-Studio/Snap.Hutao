// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Core.Setting;

/// <summary>
/// 功能
/// </summary>
internal sealed class Feature : ObservableObject
{
    private readonly string displayName;
    private readonly string description;
    private readonly string settingKey;
    private readonly bool defaultValue;

    /// <summary>
    /// 构造一个新的功能
    /// </summary>
    /// <param name="displayName">显示名称</param>
    /// <param name="description">描述</param>
    /// <param name="settingKey">键</param>
    /// <param name="defaultValue">默认值</param>
    public Feature(string displayName, string description, string settingKey, bool defaultValue)
    {
        this.displayName = displayName;
        this.description = description;
        this.settingKey = settingKey;
        this.defaultValue = defaultValue;
    }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get => displayName; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get => description; }

    /// <summary>
    /// 值
    /// </summary>
    public bool Value
    {
        get => LocalSetting.Get(settingKey, defaultValue);
        set
        {
            LocalSetting.Set(settingKey, value);
            OnPropertyChanged(nameof(Value));
        }
    }
}