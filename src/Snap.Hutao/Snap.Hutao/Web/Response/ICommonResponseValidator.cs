// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

internal interface ICommonResponseValidator<in TResponse>
    where TResponse : ICommonResponse<TResponse>
{
    bool TryValidate(TResponse response);

    bool TryValidateWithoutUINotification(TResponse response);
}
