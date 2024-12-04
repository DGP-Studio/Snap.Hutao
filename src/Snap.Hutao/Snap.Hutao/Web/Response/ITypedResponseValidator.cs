// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

internal interface ITypedResponseValidator<TData> : ICommonResponseValidator<Response<TData>>
{
    bool TryValidate(Response<TData> response, [NotNullWhen(true)] out TData? data);

    bool TryValidateWithoutUINotification(Response<TData> response, [NotNullWhen(true)] out TData? data);
}
