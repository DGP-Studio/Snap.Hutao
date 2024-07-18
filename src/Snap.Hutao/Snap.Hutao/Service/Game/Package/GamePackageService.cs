// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.Game;

namespace Snap.Hutao.Service.Game.Package;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePackageService))]
internal sealed partial class GamePackageService : IGamePackageService
{
    private readonly LaunchOptions launchOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ITaskContext taskContext;

    public GamePackageServiceState State { get; set; }

    public async ValueTask StartOperationAsync()
    {
        GamePackageOperationWindow window = serviceProvider.GetRequiredService<GamePackageOperationWindow>();
        switch (State)
        {
            case GamePackageServiceState.Verify:
                await VerifyAsync(window).ConfigureAwait(false);
                break;
            case GamePackageServiceState.Update:
                await UpdateAsync(window).ConfigureAwait(false);
                break;
            case GamePackageServiceState.Predownload:
                await PredownloadAsync(window).ConfigureAwait(false);
                break;
            default:
                break;
        }
    }

    public async ValueTask CancelOperationAsync()
    {

    }

    private async ValueTask VerifyAsync(GamePackageOperationWindow window)
    {
        GamePackageOperationViewModel viewModel = new("正在验证游戏完整性");
        window.InitializeDataContext(viewModel);
        await Task.Delay(3000).ConfigureAwait(true);

    }

    private async ValueTask UpdateAsync(GamePackageOperationWindow window)
    {
        GamePackageOperationViewModel viewModel = new("正在更新游戏");
        window.InitializeDataContext(viewModel);
        await Task.Delay(3000).ConfigureAwait(true);

    }

    private async ValueTask PredownloadAsync(GamePackageOperationWindow window)
    {
        GamePackageOperationViewModel viewModel = new("正在预下载资源");
        window.InitializeDataContext(viewModel);
        await Task.Delay(3000).ConfigureAwait(true);
    }
}
