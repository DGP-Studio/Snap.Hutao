// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Redeem;

internal sealed class RedeemGenerateRequest
{
    public required uint Count { get; set; }

    public required RedeemCodeType Type { get; set; }

    public required RedeemCodeTargetServiceType ServiceType { get; set; }

    public required int Value { get; set; }

    public required string Description { get; set; }

    public required DateTimeOffset ExpireTime { get; set; }

    public required uint Times { get; set; }
}