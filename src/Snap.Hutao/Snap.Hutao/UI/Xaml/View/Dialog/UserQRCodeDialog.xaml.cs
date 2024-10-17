// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Factory.QuickResponse;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hoyolab.Hk4e.Sdk.Combo;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[ConstructorGenerated(InitializeComponent = true)]
[DependencyProperty("QRCodeSource", typeof(ImageSource))]
internal sealed partial class UserQRCodeDialog : ContentDialog, IDisposable
{
    private readonly IInfoBarService infoBarService;
    private readonly IQRCodeFactory qrCodeFactory;
    private readonly ITaskContext taskContext;
    private readonly PandaClient pandaClient;

    private readonly CancellationTokenSource userManualCancellationTokenSource = new();
    private bool disposed;

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

    public async ValueTask<ValueResult<bool, UidGameToken>> GetUidGameTokenAsync()
    {
        try
        {
            return await GetUidGameTokenCoreAsync().ConfigureAwait(false);
        }
        finally
        {
            userManualCancellationTokenSource.Dispose();
        }
    }

    [Command("CancelCommand")]
    private void Cancel()
    {
        userManualCancellationTokenSource.Cancel();
    }

    private async ValueTask<ValueResult<bool, UidGameToken>> GetUidGameTokenCoreAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        _ = ShowAsync();

        while (!userManualCancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                CancellationToken token = userManualCancellationTokenSource.Token;
                string ticket = await FetchQRCodeAndSetImageAsync(token).ConfigureAwait(false);
                UidGameToken? uidGameToken = await WaitQueryQRCodeConfirmAsync(ticket, token).ConfigureAwait(false);

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

        return new(false, default!);
    }

    private async ValueTask<string> FetchQRCodeAndSetImageAsync(CancellationToken token)
    {
        Response<UrlWrapper> fetchResponse = await pandaClient.QRCodeFetchAsync(token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(fetchResponse, infoBarService, out UrlWrapper? wrapper))
        {
            return string.Empty;
        }

        string url = wrapper.Url;
        string ticket = GetTicketFromUrl(url);

        await taskContext.SwitchToMainThreadAsync();

        BitmapImage bitmap = new();
        await bitmap.SetSourceAsync(new MemoryStream(qrCodeFactory.Create(url)).AsRandomAccessStream());
        QRCodeSource = bitmap;

        return ticket;

        static string GetTicketFromUrl(in ReadOnlySpan<char> urlSpan)
        {
            ReadOnlySpan<char> querySpan = urlSpan[urlSpan.IndexOf('?')..];
            NameValueCollection queryCollection = HttpUtility.ParseQueryString(querySpan.ToString());
            return queryCollection.TryGetSingleValue("ticket", out string? ticket) ? ticket : string.Empty;
        }
    }

    private async ValueTask<UidGameToken?> WaitQueryQRCodeConfirmAsync(string ticket, CancellationToken token)
    {
        using (PeriodicTimer timer = new(new(0, 0, 3)))
        {
            while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                Response<GameLoginResult> query = await pandaClient.QRCodeQueryAsync(ticket, token).ConfigureAwait(false);

                if (query is { ReturnCode: 0, Data: { Stat: "Confirmed", Payload.Proto: "Account" } })
                {
                    UidGameToken? uidGameToken = JsonSerializer.Deserialize<UidGameToken>(query.Data.Payload.Raw);
                    ArgumentNullException.ThrowIfNull(uidGameToken);
                    return uidGameToken;
                }

                if (query.ReturnCode is (int)KnownReturnCode.QrCodeExpired)
                {
                    break;
                }
            }
        }

        return null;
    }
}