// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Core.Diagnostics;

internal interface IProcess : IDisposable
{
    public int Id { get; }

    public nint Handle { get; }

    public HWND MainWindowHandle { get; }

    public bool HasExited { get; }

    public int ExitCode { get; }

    public void Start();

    public void ResumeMainThread();

    public void WaitForExit();

    public void Kill();
}