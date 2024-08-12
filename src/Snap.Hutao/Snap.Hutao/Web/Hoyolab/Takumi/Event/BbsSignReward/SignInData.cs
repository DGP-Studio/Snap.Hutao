// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Endpoint.Hoyolab;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 签到提交数据
/// </summary>
internal sealed class SignInData
{
    /// <summary>
    /// 构造一个新的签到提交数据
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="isOversea">是否为国际服，用于选择ActivityId</param>
    public SignInData(PlayerUid uid, bool isOversea)
    {
        ActivityId = isOversea ? ApiOsEndpoints.SignInRewardActivityId : ApiEndpoints.LunaActivityId;
        Region = uid.Region;
        Uid = uid.Value;
    }

    /// <summary>
    /// 活动Id
    /// </summary>
    [JsonPropertyName("act_id")]
    public string ActivityId { get; }

    /// <summary>
    /// 地区代码
    /// </summary>
    [JsonPropertyName("region")]
    public Region Region { get; }

    /// <summary>
    /// Uid
    /// </summary>
    [JsonPropertyName("uid")]
    public string Uid { get; }
}