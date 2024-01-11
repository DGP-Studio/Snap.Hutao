// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Windows.Win32.Foundation;
using Windows.Win32.System.Console;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Logging;

internal sealed class ConsoleWindowLifeTime : IDisposable
{
    private readonly bool consoleWindowAllocated;

    public ConsoleWindowLifeTime()
    {
        if (LocalSetting.Get(SettingKeys.IsAllocConsoleDebugModeEnabled2, false))
        {
            consoleWindowAllocated = AllocConsole();
            if (consoleWindowAllocated)
            {
                HANDLE inputHandle = GetStdHandle(STD_HANDLE.STD_INPUT_HANDLE);
                if (GetConsoleMode(inputHandle, out CONSOLE_MODE mode))
                {
                    mode &= ~CONSOLE_MODE.ENABLE_QUICK_EDIT_MODE;
                    SetConsoleMode(inputHandle, mode);
                }

                SetConsoleTitle("Snap Hutao Debug Console");
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