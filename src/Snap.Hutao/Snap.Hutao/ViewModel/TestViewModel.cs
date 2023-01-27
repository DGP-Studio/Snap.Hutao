// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
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
    /// <summary>
    /// 构造一个新的测试视图模型
    /// </summary>
    public TestViewModel()
    {
        ShowCommunityGameRecordDialogCommand = new AsyncRelayCommand(ShowCommunityGameRecordDialogAsync);
        ShowAdoptCalculatorDialogCommand = new AsyncRelayCommand(ShowAdoptCalculatorDialogAsync);
        DangerousLoginMihoyoBbsCommand = new AsyncRelayCommand(DangerousLoginMihoyoBbsAsync);
        DownloadStaticFileCommand = new AsyncRelayCommand(DownloadStaticFileAsync);
        HutaoDatabasePresentCommand = new RelayCommand(HutaoDatabasePresent);
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
    /// Dangerous 登录米游社命令
    /// </summary>
    public ICommand DangerousLoginMihoyoBbsCommand { get; }

    /// <summary>
    /// 下载资源文件命令
    /// </summary>
    public ICommand DownloadStaticFileCommand { get; }

    /// <summary>
    /// 胡桃数据库呈现命令
    /// </summary>
    public ICommand HutaoDatabasePresentCommand { get; }

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

    private async Task DangerousLoginMihoyoBbsAsync()
    {
        // ContentDialog must be created by main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        (bool isOk, Dictionary<string, string>? data) = await new LoginMihoyoBBSDialog().GetInputAccountPasswordAsync().ConfigureAwait(false);

        if (isOk)
        {
            (Response<LoginResult>? resp, Aigis? aigis) = await Ioc.Default
                .GetRequiredService<PassportClient2>()
                .LoginByPasswordAsync(data, CancellationToken.None)
                .ConfigureAwait(false);

            if (resp != null)
            {
                if (resp.IsOk())
                {
                    Cookie cookie = Cookie.FromLoginResult(resp.Data);

                    await Ioc.Default
                        .GetRequiredService<IUserService>()
                        .ProcessInputCookieAsync(cookie)
                        .ConfigureAwait(false);
                }

                if (resp.ReturnCode == (int)KnownReturnCode.RET_NEED_AIGIS)
                {
                }
            }
        }
    }

    private async Task DownloadStaticFileAsync()
    {
        BitsManager bitsManager = Ioc.Default.GetRequiredService<BitsManager>();
        Uri testUri = new(Web.HutaoEndpoints.StaticZip("AvatarIcon"));
        ILogger<TestViewModel> logger = Ioc.Default.GetRequiredService<ILogger<TestViewModel>>();
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

    private void HutaoDatabasePresent()
    {
        Ioc.Default
            .GetRequiredService<Service.Navigation.INavigationService>()
            .Navigate<View.Page.HutaoDatabasePresentPage>(Service.Navigation.INavigationAwaiter.Default);
    }
}