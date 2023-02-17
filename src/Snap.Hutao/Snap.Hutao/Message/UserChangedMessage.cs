// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.User;

namespace Snap.Hutao.Message;

/// <summary>
/// 用户切换消息
/// </summary>
[HighQuality]
internal sealed class UserChangedMessage : ValueChangedMessage<User>
{
}