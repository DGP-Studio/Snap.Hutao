// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Bits;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 测试视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class TestViewModel : Abstraction.ViewModel
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
}