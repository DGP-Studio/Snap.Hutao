// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Logging;

internal sealed class ConsoleWindowLifeTime : IDisposable
{
    private readonly bool consoleWindowAllocated;

    public ConsoleWindowLifeTime()
    {
        if (LocalSetting.Get(SettingKeys.IsAllocConsoleDebugModeEnabled, false))
        {
            consoleWindowAllocated = AllocConsole();
            SetConsoleTitle("Snap Hutao Debug Console");
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