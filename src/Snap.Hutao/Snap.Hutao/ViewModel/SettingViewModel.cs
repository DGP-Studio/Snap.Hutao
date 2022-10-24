// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 测试视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class SettingViewModel : ObservableObject
{
    private readonly AppDbContext appDbContext;
    private readonly SettingEntry isEmptyHistoryWishVisibleEntry;

    private bool isEmptyHistoryWishVisible;

    /// <summary>
    /// 构造一个新的测试视图模型
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="experimental">实验性功能</param>
    public SettingViewModel(AppDbContext appDbContext, ExperimentalFeaturesViewModel experimental)
    {
        this.appDbContext = appDbContext;
        Experimental = experimental;

        isEmptyHistoryWishVisibleEntry = appDbContext.Settings
            .SingleOrAdd(e => e.Key == SettingEntry.IsEmptyHistoryWishVisible, () => new(SettingEntry.IsEmptyHistoryWishVisible, true.ToString()), out _);
        IsEmptyHistoryWishVisible = bool.Parse(isEmptyHistoryWishVisibleEntry.Value!);
    }

    /// <summary>
    /// 版本
    /// </summary>
    public string AppVersion
    {
        get => Core.CoreEnvironment.Version.ToString();
    }

    /// <summary>
    /// 空的历史卡池是否可见
    /// </summary>
    public bool IsEmptyHistoryWishVisible
    {
        get => isEmptyHistoryWishVisible;
        set
        {
            SetProperty(ref isEmptyHistoryWishVisible, value);
            isEmptyHistoryWishVisibleEntry.Value = value.ToString();
            appDbContext.Settings.UpdateAndSave(isEmptyHistoryWishVisibleEntry);
        }
    }

    /// <summary>
    /// 实验性功能
    /// </summary>
    public ExperimentalFeaturesViewModel Experimental { get; }
}