// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Factory.QrCode;
using Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;
using Snap.Hutao.Web.Hoyolab.Passport;
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
    private readonly PassportClient2 passportClient2;
    private readonly IQrCodeFactory qrCodeFactory;

    private UidGameToken? account;

    /// <summary>
    /// 构造一个新的扫描二维码对话框
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public QrCodeDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        passportClient2 = serviceProvider.GetRequiredService<PassportClient2>();
        qrCodeFactory = serviceProvider.GetRequiredService<IQrCodeFactory>();

        FetchQrCodeAsync().SafeForget();
    }

    /// <summary>
    /// 获取登录的用户
    /// </summary>
    /// <returns>QrCodeAccount</returns>
    [SuppressMessage("", "SH007")]
    public async ValueTask<ValueResult<bool, UidGameToken>> GetAccountAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        ContentDialogResult result = await ShowAsync();
        return new(account is not null, account!);
    }

    private async ValueTask FetchQrCodeAsync()
    {
        Response<GameLoginRequestResult> fetch = await passportClient2.PostQrCodeFetchAsync().ConfigureAwait(false);
        if (fetch.IsOk())
        {
            string url = Regex.Unescape(fetch.Data.Url);
            string ticket = url.ToUri().Query.Split('&').Last().Split('=').Last();

            await taskContext.SwitchToMainThreadAsync();
            BitmapImage bitmap = new();
            await bitmap.SetSourceAsync(new MemoryStream(qrCodeFactory.CreateQrCodeByteArray(url)).AsRandomAccessStream());

            ImageView.Source = bitmap;
            if (bitmap is BitmapSource { PixelHeight: > 0, PixelWidth: > 0 })
            {
                VisualStateManager.GoToState(this, "Loaded", true);
            }

            await taskContext.SwitchToBackgroundAsync();
            await CheckStatusAsync(ticket).ConfigureAwait(false);
        }
    }

    private async ValueTask CheckStatusAsync(string ticket)
    {
        using (PeriodicTimer timer = new(TimeSpan.FromSeconds(3)))
        {
            while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                Response<GameLoginResult> query = await passportClient2.PostQrCodeQueryAsync(ticket).ConfigureAwait(false);
                if (query.IsOk(false))
                {
                    switch (query.Data.Stat)
                    {
                        case GameLoginResultStatus.Init:
                        case GameLoginResultStatus.Scanned:
                            break; // @switch
                        case GameLoginResultStatus.Confirmed:
                            if (query.Data.Payload.Proto == GameLoginResultPayload.ACCOUNT)
                            {
                                account = JsonSerializer.Deserialize<UidGameToken>(query.Data.Payload.Raw);
                                await taskContext.SwitchToMainThreadAsync();
                                Hide();
                                return; // Stop timer
                            }

                            break; // @switch
                    }
                }
                else if (query.ReturnCode == (int)KnownReturnCode.QrCodeExpired)
                {
                    FetchQrCodeAsync().SafeForget();
                    break; // @while
                }
            }
        }
    }
}
