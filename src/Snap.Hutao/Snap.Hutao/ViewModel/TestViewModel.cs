// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Bits;
using Snap.Hutao.View.Dialog;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 测试视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class TestViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的测试视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public TestViewModel(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        ShowCommunityGameRecordDialogCommand = new AsyncRelayCommand(ShowCommunityGameRecordDialogAsync);
        ShowAdoptCalculatorDialogCommand = new AsyncRelayCommand(ShowAdoptCalculatorDialogAsync);
        DownloadStaticFileCommand = new AsyncRelayCommand(DownloadStaticFileAsync);
        RestartAppCommand = new RelayCommand<bool>(RestartApp);
    }

    /// <summary>
    /// 打开游戏社区记录对话框命令
    /// </summary>
    public ICommand ShowCommunityGameRecordDialogCommand { get; }

    /// <summary>
    /// 打开养成计算对话框命令
    /// </summary>
    public ICommand ShowAdoptCalculatorDialogCommand { get; }

    /// <summary>
    /// 下载资源文件命令
    /// </summary>
    public ICommand DownloadStaticFileCommand { get; }

    /// <summary>
    /// 重启命令
    /// </summary>
    public ICommand RestartAppCommand { get; }

    /// <inheritdoc/>
    protected override Task OpenUIAsync()
    {
        return Task.CompletedTask;
    }

    private async Task ShowCommunityGameRecordDialogAsync()
    {
        // ContentDialog must be created by main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        await new CommunityGameRecordDialog().ShowAsync();
    }

    private async Task ShowAdoptCalculatorDialogAsync()
    {
        // ContentDialog must be created by main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        await new AdoptCalculatorDialog().ShowAsync();
    }

    private async Task DownloadStaticFileAsync()
    {
        BitsManager bitsManager = serviceProvider.GetRequiredService<BitsManager>();
        Uri testUri = new(Web.HutaoEndpoints.StaticZip("AvatarIcon"));
        ILogger<TestViewModel> logger = serviceProvider.GetRequiredService<ILogger<TestViewModel>>();
        Progress<ProgressUpdateStatus> progress = new(status => logger.LogInformation("{info}", status));
        (bool isOk, TempFile file) = await bitsManager.DownloadAsync(testUri, progress).ConfigureAwait(false);

        using (file)
        {
            if (isOk)
            {
                logger.LogInformation("Download completed.");
            }
            else
            {
                logger.LogInformation("Download failed.");
            }
        }
    }

    private void RestartApp(bool elevated)
    {
        AppInstance.Restart(string.Empty);
    }
}