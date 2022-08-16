// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding;

namespace Snap.Hutao.Message;

/// <summary>
/// 用户切换消息
/// </summary>
internal class UserChangedMessage : ValueChangedMessage<User>
{
    /// <summary>
    /// 构造一个新的用户切换消息
    /// </summary>
    /// <param name="oldUser">老用户</param>
    /// <param name="newUser">新用户</param>
    public UserChangedMessage(User? oldUser, User? newUser)
        : base(oldUser, newUser)
    {
    }
}