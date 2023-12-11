// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 扫码登录结果
/// </summary>
[HighQuality]
internal sealed class GameLoginResult
{
    [JsonPropertyName("stat")]
    [JsonEnum(JsonSerializeType.String)]
    public GameLoginResultStatus Stat { get; set; } = default!;

    [JsonPropertyName("payload")]
    public GameLoginResultPayload Payload { get; set; } = default!;
}
