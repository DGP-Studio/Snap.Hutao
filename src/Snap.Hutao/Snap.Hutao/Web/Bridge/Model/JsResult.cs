// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

internal sealed class JsResult<TData> : IJsBridgeResult
{
    [JsonPropertyName("retcode")]
    public int ReturnCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public TData Data { get; set; } = default!;
}