// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;
using System.Diagnostics;
using System.Text;

namespace Snap.Hutao.Message;

/// <summary>
/// 用户切换消息
/// </summary>
[HighQuality]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
internal sealed class UserChangedMessage : ValueChangedMessage<User>
{
    // defaults to the UserAndRoleChanged when we raise this message in ScopedDbCurrent
    public UserChangeFlag Flag { get; private set; } = UserChangeFlag.UserAndRoleChanged;

    public bool IsOnlyRoleChanged { get => Flag == UserChangeFlag.RoleChanged; }

    public static UserChangedMessage Create(User oldValue, User newValue, UserChangeFlag flag)
    {
        return new UserChangedMessage
        {
            OldValue = oldValue,
            NewValue = newValue,
            Flag = flag,
        };
    }

    public static UserChangedMessage CreateOnlyRoleChanged(User value)
    {
        return Create(value, value, UserChangeFlag.RoleChanged);
    }

#if DEBUG
    private string DebuggerDisplay()
    {
        StringBuilder stringBuilder = new();
        stringBuilder
            .Append("Name:")
            .Append(OldValue?.UserInfo?.Nickname)
            .Append("|Role[")
            .Append(OldValue?.UserGameRoles?.Count)
            .Append("]:<")
            .Append(OldValue?.SelectedUserGameRole)
            .Append("> -> Name:")
            .Append(NewValue?.UserInfo?.Nickname)
            .Append("|Role[")
            .Append(NewValue?.UserGameRoles?.Count)
            .Append("]:<")
            .Append(NewValue?.SelectedUserGameRole)
            .Append('>');

        return stringBuilder.ToString();
    }
#endif
}