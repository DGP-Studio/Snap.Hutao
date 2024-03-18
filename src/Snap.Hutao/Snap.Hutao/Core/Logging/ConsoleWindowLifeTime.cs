// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Console;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Core.Logging;

internal sealed class ConsoleWindowLifeTime : IDisposable
{
    private readonly bool consoleWindowAllocated;

    public ConsoleWindowLifeTime()
    {
        if (LocalSetting.Get(SettingKeys.IsAllocConsoleDebugModeEnabled, false))
        {
            consoleWindowAllocated = AllocConsole();
            if (consoleWindowAllocated)
            {
                HANDLE inputHandle = GetStdHandle(STD_HANDLE.STD_INPUT_HANDLE);
                if (GetConsoleMode(inputHandle, out CONSOLE_MODE mode))
                {
                    mode &= ~CONSOLE_MODE.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                    SetConsoleMode(inputHandle, mode);
                }

                SetConsoleTitleW("Snap Hutao Debug Console");
            }
        }
    }

    public void Dispose()
    {
        if (consoleWindowAllocated)
        {
            FreeConsole();
        }
    }
}