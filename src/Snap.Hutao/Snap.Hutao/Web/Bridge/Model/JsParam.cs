// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

internal sealed class JsParam
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = default!;

    [JsonPropertyName("payload")]
    public JsonElement Payload { get; set; }

    [JsonPropertyName("callback")]
    public string? Callback { get; set; }
}

[SuppressMessage("", "SA1402")]
internal sealed class JsParam<TPayload>
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = default!;

    [JsonPropertyName("payload")]
    public TPayload Payload { get; set; } = default!;

    [JsonPropertyName("callback")]
    public string? Callback { get; set; }

    public static implicit operator JsParam<TPayload>(JsParam jsParam)
    {
        return new()
        {
            Method = jsParam.Method,
            Payload = jsParam.Payload.Deserialize<TPayload>() ?? default!,
            Callback = jsParam.Callback,
        };
    }
}