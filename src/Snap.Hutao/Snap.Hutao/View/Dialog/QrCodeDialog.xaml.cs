// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Factory.QrCode;
using Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Text.RegularExpressions;

namespace Snap.Hutao.View.Dialog;

/// <summary>
/// 扫描二维码对话框
/// </summary>
[HighQuality]
internal sealed partial class QrCodeDialog : ContentDialog
{
    private readonly ITaskContext taskContext;
    private readonly QrCodeClient qrCodeClient;
    private readonly IQrCodeFactory qrCodeFactory;

    private QrCodeAccount? account;

    /// <summary>
    /// 构造一个新的扫描二维码对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public QrCodeDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        qrCodeClient = serviceProvider.GetRequiredService<QrCodeClient>();
        qrCodeFactory = serviceProvider.GetRequiredService<IQrCodeFactory>();

        Initialize().SafeForget();
    }

    /// <summary>
    /// 获取登录的用户
    /// </summary>
    /// <returns>QrCodeAccount</returns>
    public async ValueTask<ValueResult<bool, QrCodeAccount>> GetAccountAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        return new(account is not null, account!);
    }

    private async ValueTask Initialize()
    {
        Response<QrCodeFetch> fetch = await qrCodeClient.PostQrCodeFetchAsync().ConfigureAwait(false);
        if (fetch.IsOk())
        {
            string url = Regex.Unescape(fetch.Data.Url);
            string ticket = url.ToUri().Query.Split('&').Last().Split('=').Last();

            await taskContext.SwitchToMainThreadAsync();
            BitmapImage bitmap = new();
            await bitmap.SetSourceAsync(new MemoryStream(qrCodeFactory.CreateByteArr(url)).AsRandomAccessStream());

            ImageView.Source = bitmap;
            if (bitmap is BitmapSource { PixelHeight: > 0, PixelWidth: > 0 })
            {
                VisualStateManager.GoToState(this, "Loaded", true);
            }

            await taskContext.SwitchToBackgroundAsync();

            using (PeriodicTimer timer = new(TimeSpan.FromSeconds(3)))
            {
                while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
                {
                    Response<QrCodeQuery> query = await qrCodeClient.PostQrCodeQueryAsync(ticket).ConfigureAwait(false);
                    if (query.IsOk(false))
                    {
                        switch (query.Data.Stat)
                        {
                            case QrCodeQueryStatus.INIT:
                            case QrCodeQueryStatus.SCANNED:
                                break; // @switch
                            case QrCodeQueryStatus.CONFIRMED:
                                if (query.Data.Payload.Proto == QrCodeQueryPayload.ACCOUNT)
                                {
                                    account = JsonSerializer.Deserialize<QrCodeAccount>(query.Data.Payload.Raw);
                                    await taskContext.SwitchToMainThreadAsync();
                                    Hide();
                                    return;
                                }

                                break; // @switch
                        }
                    }
                    else if (query.ReturnCode == (int)KnownReturnCode.QrCodeExpired)
                    {
                        Initialize().SafeForget();
                        break; // @while
                    }
                }
            }
        }
    }
}
