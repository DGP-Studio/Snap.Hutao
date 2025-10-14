// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Yae.Achievement;

internal sealed class MethodRva
{
    [JsonPropertyName("doCmd")]
    public required uint DoCmd { get; init; }

    [JsonPropertyName("toUint16")]
    public required uint ToUInt16 { get; init; }

    [JsonPropertyName("updateNormalProp")]
    public required uint UpdateNormalProperty { get; init; }

    [JsonPropertyName("newString")]
    public required uint NewString { get; init; }

    [JsonPropertyName("findGameObject")]
    public required uint FindGameObject { get; init; }

    [JsonPropertyName("eventSystemUpdate")]
    public required uint EventSystemUpdate { get; init; }

    [JsonPropertyName("simulatePointerClick")]
    public required uint SimulatePointerClick { get; init; }
}