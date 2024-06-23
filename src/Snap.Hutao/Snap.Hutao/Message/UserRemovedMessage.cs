// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Message;

/// <summary>
/// 用户删除消息
/// </summary>
[HighQuality]
[Obsolete]
internal sealed class UserRemovedMessage
{
    /// <summary>
    /// 构造一个新的用户删除消息
    /// </summary>
    /// <param name="user">用户</param>
    public UserRemovedMessage(User user)
    {
        RemovedUserId = user.InnerId;
    }

    /// <summary>
    /// 删除的用户Id
    /// </summary>
    public Guid RemovedUserId { get; }
}