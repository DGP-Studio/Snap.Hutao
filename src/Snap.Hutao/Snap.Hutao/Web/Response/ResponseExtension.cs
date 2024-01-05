// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Web.Response;

internal static class ResponseExtension
{
    public static bool TryGetData<TData>(this Response<TData> response, [NotNullWhen(true)] out TData? data, IInfoBarService? infoBarService = null, IServiceProvider? serviceProvider = null)
    {
        if (response.ReturnCode == 0)
        {
            ArgumentNullException.ThrowIfNull(response.Data);
            data = response.Data;
            return true;
        }
        else
        {
            serviceProvider ??= Ioc.Default;
            infoBarService ??= serviceProvider.GetRequiredService<IInfoBarService>();
            infoBarService.Error(response.ToString());
            data = default;
            return false;
        }
    }

    public static bool TryGetDataWithoutUINotification<TData>(this Response<TData> response, [NotNullWhen(true)] out TData? data)
    {
        if (response.ReturnCode == 0)
        {
            ArgumentNullException.ThrowIfNull(response.Data);
            data = response.Data;
            return true;
        }
        else
        {
            data = default;
            return false;
        }
    }
}