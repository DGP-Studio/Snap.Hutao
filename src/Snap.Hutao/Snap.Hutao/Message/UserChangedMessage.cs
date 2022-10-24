// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding;

namespace Snap.Hutao.Message;

/// <summary>
/// 用户切换消息
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
internal class UserChangedMessage : ValueChangedMessage<User>
{
}