// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;

internal sealed class VerificationResult
{
    [JsonPropertyName("challenge")]
    public string? Challenge { get; set; }
}