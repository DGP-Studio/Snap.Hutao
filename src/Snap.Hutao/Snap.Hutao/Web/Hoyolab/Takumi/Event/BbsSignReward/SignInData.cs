// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Endpoint.Hoyolab;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class SignInData
{
    public SignInData(IApiEndpoints apiEndpoints, PlayerUid uid)
    {
        ActivityId = apiEndpoints.LunaSolActivityId();
        Region = uid.Region;
        Uid = uid.Value;
    }

    [JsonPropertyName("act_id")]
    public string ActivityId { get; }

    [JsonPropertyName("region")]
    public Region Region { get; }

    [JsonPropertyName("uid")]
    public string Uid { get; }
}