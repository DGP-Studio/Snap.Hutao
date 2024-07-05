// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Factory.QrCode;
using Snap.Hutao.Web.Hoyolab.Hk4e.Sdk.Combo;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;
using System.Collections.Specialized;
using System.IO;
using System.Web;

namespace Snap.Hutao.UI.Xaml.View.Dialog;

[DependencyProperty("QRCodeSource", typeof(ImageSource))]
internal sealed partial class UserQRCodeDialog : ContentDialog, IDisposable
{
    private readonly ITaskContext taskContext;
    private readonly PandaClient pandaClient;
    private readonly IQRCodeFactory qrCodeFactory;

    private readonly CancellationTokenSource userManualCancellationTokenSource = new();
    private bool disposed;

    public UserQRCodeDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        pandaClient = serviceProvider.GetRequiredService<PandaClient>();
        qrCodeFactory = serviceProvider.GetRequiredService<IQRCodeFactory>();
    }

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
        if (!fetchResponse.IsOk())
        {
            return string.Empty;
        }

        string url = fetchResponse.Data.Url;
        string ticket = GetTicketFromUrl(fetchResponse.Data.Url);

        await taskContext.SwitchToMainThreadAsync();

        BitmapImage bitmap = new();
        await bitmap.SetSourceAsync(new MemoryStream(qrCodeFactory.Create(url)).AsRandomAccessStream());
        QRCodeSource = bitmap;

        return ticket;

        static string GetTicketFromUrl(in ReadOnlySpan<char> urlSpan)
        {
            ReadOnlySpan<char> querySpan = urlSpan[urlSpan.IndexOf('?')..];
            NameValueCollection queryCollection = HttpUtility.ParseQueryString(querySpan.ToString());
            if (queryCollection.TryGetSingleValue("ticket", out string? ticket))
            {
                return ticket;
            }

            return string.Empty;
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
                else if (query.ReturnCode == (int)KnownReturnCode.QrCodeExpired)
                {
                    break;
                }
            }
        }

        return null;
    }
}