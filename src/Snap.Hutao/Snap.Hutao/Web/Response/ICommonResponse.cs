// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

internal interface ICommonResponse<TResponse>
    where TResponse : ICommonResponse<TResponse>
{
    int ReturnCode { get; }

    string Message { get; set; }

    static abstract TResponse CreateDefault(int returnCode, string message);
}