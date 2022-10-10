// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 玩家信息
/// </summary>
public class Player
{
    /// <summary>
    /// 昵称
    /// </summary>
    public string Nickname { get; set; } = default!;

    /// <summary>
    /// 等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 签名
    /// </summary>
    public string Signature { get; set; } = default!;

    /// <summary>
    /// 完成成就数
    /// </summary>
    public int FinishAchievementNumber { get; set; }

    /// <summary>
    /// 深渊层间
    /// </summary>
    public string SipralAbyssFloorLevel { get; set; } = default!;
}