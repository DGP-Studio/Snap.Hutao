// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

/// <summary>
/// 记录
/// </summary>
[HighQuality]
internal sealed class SimpleRecord
{
    /// <summary>
    /// 构造一个新的记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="characters">详细的角色信息</param>
    /// <param name="spiralAbyss">深渊信息</param>
    /// <param name="reservedUserName">用户名</param>
    public SimpleRecord(string uid, List<Character> characters, Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss, string? reservedUserName)
    {
        Uid = uid;
        Identity = "Snap Hutao"; // hardcoded Identity name
        SpiralAbyss = new(spiralAbyss);
        Avatars = characters.Select(a => new SimpleAvatar(a));
        ReservedUserName = reservedUserName;
    }

    /// <summary>
    /// Uid
    /// </summary>
    public string Uid { get; set; } = default!;

    /// <summary>
    /// 上传者身份
    /// </summary>
    public string Identity { get; set; } = default!;

    /// <summary>
    /// 保留属性
    /// 用户名称
    /// </summary>
    public string? ReservedUserName { get; set; }

    /// <summary>
    /// 深境螺旋
    /// </summary>
    public SimpleSpiralAbyss SpiralAbyss { get; set; } = default!;

    /// <summary>
    /// 角色
    /// </summary>
    public IEnumerable<SimpleAvatar> Avatars { get; set; } = default!;
}
