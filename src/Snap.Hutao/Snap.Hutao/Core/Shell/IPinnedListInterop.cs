// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
namespace Snap.Hutao.Core.Shell;

internal interface IPinnedListInterop
{
    bool TryPinShortcut(string shortcutPath);

    bool TryUnpinShortcut(string shortcutPath);
}