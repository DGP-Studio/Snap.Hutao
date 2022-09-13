// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Message;

/// <summary>
/// 祈愿记录存档切换消息
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
internal class GachaArchiveChangedMessage : ValueChangedMessage<GachaArchive>
{
    /// <summary>
    /// 构造一个新的用户切换消息
    /// </summary>
    /// <param name="oldArchive">老用户</param>
    /// <param name="newArchive">新用户</param>
    public GachaArchiveChangedMessage(GachaArchive? oldArchive, GachaArchive? newArchive)
        : base(oldArchive, newArchive)
    {
    }
}