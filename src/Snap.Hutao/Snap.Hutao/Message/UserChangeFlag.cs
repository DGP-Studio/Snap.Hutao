// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Message;

internal enum UserChangeFlag
{
    // This flag is impossible to appear alone
    UserChanged = 0b0001,
    RoleChanged = 0b0010,
    UserAndRoleChanged = UserChanged | RoleChanged,
}