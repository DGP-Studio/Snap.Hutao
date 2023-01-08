// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Control;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Bits;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
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
internal class TestViewModel : ObservableObject, ISupportCancellation
{
    /// <summary>
    /// 构造一个新的测试视图模型
    /// </summary>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public TestViewModel(IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        ShowCommunityGameRecordDialogCommand = asyncRelayCommandFactory.Create(ShowCommunityGameRecordDialogAsync);
        DangerousLoginMihoyoBbsCommand = asyncRelayCommandFactory.Create(DangerousLoginMihoyoBbsAsync);
        DownloadStaticFileCommand = asyncRelayCommandFactory.Create(DownloadStaticFileAsync);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 打开游戏社区记录对话框命令
    /// </summary>
    public ICommand ShowCommunityGameRecordDialogCommand { get; }

    /// <summary>
    /// Dangerous 登录米游社命令
    /// </summary>
    public ICommand DangerousLoginMihoyoBbsCommand { get; }

    /// <summary>
    /// 下载资源文件命令
    /// </summary>
    public ICommand DownloadStaticFileCommand { get; }

    private async Task ShowCommunityGameRecordDialogAsync()
    {
        CommunityGameRecordDialog dialog = ActivatorUtilities.CreateInstance<CommunityGameRecordDialog>(Ioc.Default);
        await dialog.ShowAsync();
    }

    private async Task DangerousLoginMihoyoBbsAsync()
    {
        LoginMihoyoBBSDialog dialog = ActivatorUtilities.CreateInstance<LoginMihoyoBBSDialog>(Ioc.Default);
        (bool isOk, Dictionary<string, string>? data) = await dialog.GetInputAccountPasswordAsync().ConfigureAwait(false);

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
}