// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Control;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.LaunchGame;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 启动游戏视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class LaunchGameViewModel : ObservableObject, ISupportCancellation
{
    private readonly IGameService gameService;

    private readonly List<LaunchScheme> knownSchemes = new()
    {
        new LaunchScheme(name: "官方服 | 天空岛", channel: "1", subChannel: "1"),
        new LaunchScheme(name: "渠道服 | 世界树", channel: "14", subChannel: "0"),

        // new LaunchScheme(name: "国际服 | 暂不支持", channel: "1", subChannel: "0"),
    };

    private LaunchScheme? selectedScheme;
    private ObservableCollection<GameAccount>? gameAccounts;
    private GameAccount? selectedGameAccount;
    private bool isFullScreen;
    private bool isBorderless;
    private int screenWidth;
    private int screenHeight;
    private bool unlockFps;
    private int targetFps;

    /// <summary>
    /// 构造一个新的启动游戏视图模型
    /// </summary>
    /// <param name="gameService">游戏服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public LaunchGameViewModel(IGameService gameService, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.gameService = gameService;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        LaunchCommand = asyncRelayCommandFactory.Create(LaunchAsync);
        DetectGameAccountCommand = asyncRelayCommandFactory.Create(DetectGameAccountAsync);
        ModifyGameAccountCommand = asyncRelayCommandFactory.Create(ModifyGameAccountAsync);
        RemoveGameAccountCommand = asyncRelayCommandFactory.Create(RemoveGameAccountAsync);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 已知的服务器方案
    /// </summary>
    public List<LaunchScheme> KnownSchemes { get => knownSchemes; }

    /// <summary>
    /// 当前选择的服务器方案
    /// </summary>
    public LaunchScheme? SelectedScheme { get => selectedScheme; set => SetProperty(ref selectedScheme, value); }

    /// <summary>
    /// 游戏账号集合
    /// </summary>
    public ObservableCollection<GameAccount>? GameAccounts { get => gameAccounts; set => SetProperty(ref gameAccounts, value); }

    /// <summary>
    /// 选中的账号
    /// </summary>
    public GameAccount? SelectedGameAccount { get => selectedGameAccount; set => SetProperty(ref selectedGameAccount, value); }

    /// <summary>
    /// 全屏
    /// </summary>
    public bool IsFullScreen { get => isFullScreen; set => SetProperty(ref isFullScreen, value); }

    /// <summary>
    /// 无边框
    /// </summary>
    public bool IsBorderless { get => isBorderless; set => SetProperty(ref isBorderless, value); }

    /// <summary>
    /// 宽度
    /// </summary>
    public int ScreenWidth { get => screenWidth; set => SetProperty(ref screenWidth, value); }

    /// <summary>
    /// 高度
    /// </summary>
    public int ScreenHeight { get => screenHeight; set => SetProperty(ref screenHeight, value); }

    /// <summary>
    /// 解锁帧率
    /// </summary>
    public bool UnlockFps { get => unlockFps; set => SetProperty(ref unlockFps, value); }

    /// <summary>
    /// 目标帧率
    /// </summary>
    public int TargetFps { get => targetFps; set => SetProperty(ref targetFps, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 启动游戏命令
    /// </summary>
    public ICommand LaunchCommand { get; }

    /// <summary>
    /// 检测游戏账号命令
    /// </summary>
    public ICommand DetectGameAccountCommand { get; }

    /// <summary>
    /// 修改游戏账号命令
    /// </summary>
    public ICommand ModifyGameAccountCommand { get; }

    /// <summary>
    /// 删除游戏账号命令
    /// </summary>
    public ICommand RemoveGameAccountCommand { get; }

    private async Task OpenUIAsync()
    {
        (bool isOk, string gamePath) = await gameService.GetGamePathAsync().ConfigureAwait(false);

        if (isOk)
        {
            MultiChannel multi = gameService.GetMultiChannel();
            SelectedScheme = KnownSchemes.FirstOrDefault(s => s.Channel == multi.Channel && s.SubChannel == multi.SubChannel);
        }
    }

    private async Task LaunchAsync()
    {

    }

    private async Task DetectGameAccountAsync()
    {

    }

    private async Task ModifyGameAccountAsync()
    {

    }

    private async Task RemoveGameAccountAsync()
    {

    }
}