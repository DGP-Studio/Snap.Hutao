// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Web.Response;

internal sealed class DefaultResponseValidator : ICommonResponseValidator<Response>
{
    private readonly IInfoBarService infoBarService;

    public DefaultResponseValidator(IInfoBarService infoBarService)
    {
        this.infoBarService = infoBarService;
    }

    public bool TryValidate(Response response)
    {
        if (response.ReturnCode is 0)
        {
            return true;
        }

        infoBarService.Error(response.ToString());
        return false;
    }

    public bool TryValidateWithoutUINotification(Response response)
    {
        return response.ReturnCode is 0;
    }
}
