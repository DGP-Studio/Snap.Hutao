// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Web.Response;

[SuppressMessage("", "SA1402")]
internal sealed class TypedResponseValidator<TData> : ITypedResponseValidator<TData>
{
    private readonly IInfoBarService infoBarService;

    public TypedResponseValidator(IInfoBarService infoBarService)
    {
        this.infoBarService = infoBarService;
    }

    public bool TryValidate(Response<TData> response, [NotNullWhen(true)] out TData? data)
    {
        if (TryValidate(response))
        {
            ArgumentNullException.ThrowIfNull(response.Data);
            data = response.Data;
            return true;
        }

        data = default;
        return false;
    }

    public bool TryValidate(Response<TData> response)
    {
        if (response.ReturnCode is 0)
        {
            return true;
        }

        infoBarService.Error(response.ToString());
        return false;
    }

    public bool TryValidateWithoutUINotification(Response<TData> response, [NotNullWhen(true)] out TData? data)
    {
        if (TryValidateWithoutUINotification(response))
        {
            ArgumentNullException.ThrowIfNull(response.Data);
            data = response.Data;
            return true;
        }

        data = default;
        return false;
    }

    public bool TryValidateWithoutUINotification(Response<TData> response)
    {
        return response.ReturnCode is 0;
    }
}