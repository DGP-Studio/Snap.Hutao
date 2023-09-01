// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Hutao;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao;

internal static class HomaPassportHttpClientAuthorizationExtension
{
    public static async ValueTask TrySetTokenAsync(this HttpClient httpClient, HutaoUserOptions hutaoUserOptions)
    {
        string? token = await hutaoUserOptions.GetTokenAsync().ConfigureAwait(false);
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }
}