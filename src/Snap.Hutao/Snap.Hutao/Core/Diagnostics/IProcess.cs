// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Core.Diagnostics;

internal interface IProcess : IDisposable
{
    int Id { get; }

    nint Handle { get; }

    HWND MainWindowHandle { get; }

    bool HasExited { get; }

    int ExitCode { get; }

    void Start();

    void ResumeMainThread();

    void WaitForExit();

    void Kill();
}