// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

internal sealed class ActionTypePayload
{
    [JsonPropertyName("action_type")]
    public string ActionType { get; set; } = default!;
}