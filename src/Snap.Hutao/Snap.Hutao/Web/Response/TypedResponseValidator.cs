// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Web.Response;

[SuppressMessage("", "SA1402")]
internal sealed class TypedResponseValidator<TData> : ITypedResponseValidator<TData>
{
    private readonly IMessenger messenger;

    public TypedResponseValidator(IMessenger messenger)
    {
        this.messenger = messenger;
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

        messenger.Send(InfoBarMessage.Error(response.ToString()));
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