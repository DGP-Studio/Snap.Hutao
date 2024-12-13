// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using System.Text;
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

        StringBuilder titleBuilder = new StringBuilder()
            .Append("Snap Hutao Debug Console")
            .AppendIf(HutaoRuntime.IsProcessElevated, " [Administrator]")
            .Append(HutaoRuntime.Version);

        SetConsoleTitleW(titleBuilder.ToString());
    }

    public void Dispose()
    {
        if (consoleWindowAllocated)
        {
            FreeConsole();
        }
    }
}