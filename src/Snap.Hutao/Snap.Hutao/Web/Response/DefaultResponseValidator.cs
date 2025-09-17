// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Web.Response;

internal sealed class DefaultResponseValidator : ICommonResponseValidator<Response>
{
    private readonly IMessenger messenger;

    public DefaultResponseValidator(IMessenger messenger)
    {
        this.messenger = messenger;
    }

    public bool TryValidate(Response response)
    {
        if (response.ReturnCode is 0)
        {
            return true;
        }

        messenger.Send(InfoBarMessage.Error(response.ToString()));
        return false;
    }

    public bool TryValidateWithoutUINotification(Response response)
    {
        return response.ReturnCode is 0;
    }
}