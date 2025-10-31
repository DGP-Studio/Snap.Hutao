// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.QuickResponse;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.IO;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty<ImageSource>("QRCodeSource")]
internal sealed partial class UserQRCodeDialog : ContentDialog, IDisposable
{
    private readonly HoyoPlayPassportClient hoyoPlayPassportClient;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IQRCodeFactory qrCodeFactory;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private readonly CancellationTokenSource userManualCancellationTokenSource = new();
    private bool disposed;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial UserQRCodeDialog(IServiceProvider serviceProvider);

    ~UserQRCodeDialog()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (!disposed)
        {
            userManualCancellationTokenSource.Dispose();
            disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    public async ValueTask<ValueResult<bool, QrLoginResult?>> GetQrLoginResultAsync()
    {
        try
        {
            return await PrivateGetQRLoginResultAsync().ConfigureAwait(false);
        }
        finally
        {
            userManualCancellationTokenSource.Dispose();
        }
    }

    [Command("CancelCommand")]
    private void Cancel()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Cancel", "UserQRCodeDialog.Command"));
        userManualCancellationTokenSource.Cancel();
    }

    private async ValueTask<ValueResult<bool, QrLoginResult?>> PrivateGetQRLoginResultAsync()
    {
        await contentDialogFactory.EnqueueAndShowAsync(this).QueueTask.ConfigureAwait(false);

        while (!userManualCancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                CancellationToken token = userManualCancellationTokenSource.Token;
                string ticket = await FetchQRCodeAndSetImageAsync(token).ConfigureAwait(false);
                QrLoginResult? uidGameToken = await WaitQueryQRCodeConfirmAsync(ticket, token).ConfigureAwait(false);

                if (uidGameToken is null)
                {
                    continue;
                }

                await taskContext.SwitchToMainThreadAsync();
                Hide();
                return new(true, uidGameToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        return new(false, default);
    }

    private async ValueTask<string> FetchQRCodeAndSetImageAsync(CancellationToken token)
    {
        Response<QrLogin> qrLoginResponse = await hoyoPlayPassportClient.CreateQrLoginAsync(token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(qrLoginResponse, messenger, out QrLogin? qrLogin))
        {
            return string.Empty;
        }

        await taskContext.SwitchToMainThreadAsync();

        BitmapImage bitmap = new();
        await bitmap.SetSourceAsync(new MemoryStream(qrCodeFactory.Create(qrLogin.Url)).AsRandomAccessStream());
        QRCodeSource = bitmap;

        return qrLogin.Ticket;
    }

    private async ValueTask<QrLoginResult?> WaitQueryQRCodeConfirmAsync(string ticket, CancellationToken token)
    {
        using (PeriodicTimer timer = new(TimeSpan.FromSeconds(3)))
        {
            while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                Response<QrLoginResult> query = await hoyoPlayPassportClient.QueryQrLoginStatusAsync(ticket, token).ConfigureAwait(false);

                if (query is { ReturnCode: 0, Data: { Status: "Confirmed", Tokens: [{ TokenType: 1 }] } })
                {
                    return query.Data;
                }

                if (query.ReturnCode is (int)KnownReturnCode.QRLoginExpired)
                {
                    break;
                }
            }
        }

        return null;
    }
}