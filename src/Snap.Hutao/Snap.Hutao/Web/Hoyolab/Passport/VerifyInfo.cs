// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Text.Json.Annotation;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class VerifyInfo
{
    [JsonPropertyName("status")]
    [JsonEnumHandling(JsonEnumHandling.String)]
    public required VerifyStatus Status { get; set; }

    [JsonPropertyName("verify_method_combinations")]
    public required ImmutableArray<VerifyMethodsWrapper> VerifyMethodCombinations { get; set; }

    [JsonPropertyName("chosen_methods")]
    public required ImmutableArray<int> ChosenMethods { get; set; }

    [JsonPropertyName("partly_verified_methods")]
    public required ImmutableArray<int> PartlyVerifiedMethods { get; set; }
}