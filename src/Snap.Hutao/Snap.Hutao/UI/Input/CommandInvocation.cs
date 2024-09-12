// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Input;

internal static class CommandInvocation
{
    public static bool TryExecute(this ICommand? command, object? parameter = null)
    {
        if (command is not null && command.CanExecute(parameter))
        {
            command.Execute(parameter);
            return true;
        }

        return false;
    }
}