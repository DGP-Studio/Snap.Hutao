// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 测试视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class SettingViewModel : ObservableObject
{
    /// <summary>
    /// 构造一个新的测试视图模型
    /// </summary>
    /// <param name="experimental">实验性功能</param>
    public SettingViewModel(ExperimentalFeaturesViewModel experimental)
    {
        Experimental = experimental;
    }

    /// <summary>
    /// 版本
    /// </summary>
    public string AppVersion
    {
        get => Core.CoreEnvironment.Version.ToString();
    }

    /// <summary>
    /// 实验性功能
    /// </summary>
    public ExperimentalFeaturesViewModel Experimental { get; }
}