// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Console;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Core.Logging;

internal sealed partial class ConsoleWindowLifeTime : IDisposable
{
    public const bool DebugModeEnabled =
#if IS_ALPHA_BUILD
        true;
#else
        false;
#endif

    private readonly bool consoleWindowAllocated;

    public ConsoleWindowLifeTime()
    {
        if (!LocalSetting.Get(SettingKeys.IsAllocConsoleDebugModeEnabled, DebugModeEnabled))
        {
            return;
        }

        consoleWindowAllocated = AllocConsole();
        if (!consoleWindowAllocated)
        {
            return;
        }

        HANDLE inputHandle = GetStdHandle(STD_HANDLE.STD_INPUT_HANDLE);
        if (GetConsoleMode(inputHandle, out CONSOLE_MODE mode))
        {
            mode |= CONSOLE_MODE.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            SetConsoleMode(inputHandle, mode);
        }

        SetConsoleTitleW(HutaoRuntime.IsProcessElevated ? "Snap Hutao Debug Console [Administrator]" : "Snap Hutao Debug Console");
    }

    public void Dispose()
    {
        if (consoleWindowAllocated)
        {
            FreeConsole();
        }
    }
}
