// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Locator;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 测试视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class SettingViewModel : ObservableObject
{
    private readonly AppDbContext appDbContext;
    private readonly IGameService gameService;
    private readonly SettingEntry isEmptyHistoryWishVisibleEntry;

    private bool isEmptyHistoryWishVisible;
    private string gamePath;

    /// <summary>
    /// 构造一个新的测试视图模型
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="gameService">游戏服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="experimental">实验性功能</param>
    public SettingViewModel(AppDbContext appDbContext, IGameService gameService, IAsyncRelayCommandFactory asyncRelayCommandFactory, ExperimentalFeaturesViewModel experimental)
    {
        this.appDbContext = appDbContext;
        this.gameService = gameService;

        Experimental = experimental;

        isEmptyHistoryWishVisibleEntry = appDbContext.Settings
            .SingleOrAdd(e => e.Key == SettingEntry.IsEmptyHistoryWishVisible, () => new(SettingEntry.IsEmptyHistoryWishVisible, true.ToString()), out _);
        IsEmptyHistoryWishVisible = bool.Parse(isEmptyHistoryWishVisibleEntry.Value!);

        GamePath = gameService.GetGamePathSkipLocator();

        SetGamePathCommand = asyncRelayCommandFactory.Create(SetGamePathAsync);
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
    /// 游戏路径
    /// </summary>
    public string GamePath
    {
        get => gamePath;
        [MemberNotNull(nameof(gamePath))]
        set => SetProperty(ref gamePath, value);
    }

    /// <summary>
    /// 实验性功能
    /// </summary>
    public ExperimentalFeaturesViewModel Experimental { get; }

    /// <summary>
    /// 设置游戏路径命令
    /// </summary>
    public ICommand SetGamePathCommand { get; }

    private async Task SetGamePathAsync()
    {
        IGameLocator locator = Ioc.Default.GetRequiredService<IEnumerable<IGameLocator>>()
            .Single(l => l.Name == nameof(ManualGameLocator));

        (bool isOk, string path) = await locator.LocateGamePathAsync().ConfigureAwait(false);
        if (isOk)
        {
            gameService.OverwriteGamePath(path);
            await ThreadHelper.SwitchToMainThreadAsync();
            GamePath = path;
        }
    }
}