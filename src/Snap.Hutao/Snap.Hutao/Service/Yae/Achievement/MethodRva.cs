// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Yae.Achievement;

internal sealed class MethodRva
{
    [JsonPropertyName("doCmd")]
    public required uint DoCmd { get; init; }

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

    [JsonPropertyName("toInt32")]
    public required uint ToInt32 { get; init; }

    [JsonPropertyName("tcpStatePtr")]
    public required uint TcpStatePtr { get; init; }

    [JsonPropertyName("sharedInfoPtr")]
    public required uint SharedInfoPtr { get; init; }

    [JsonPropertyName("decompress")]
    public required uint Decompress { get; init; }
}