// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Factory.Process;

internal sealed partial class DiagnosticsProcess : IProcess
{
    private readonly System.Diagnostics.Process process;

    public DiagnosticsProcess(System.Diagnostics.Process process)
    {
        this.process = process;
    }

    public int Id { get => process.Id; }

    public nint Handle { get => process.Handle; }

    public HWND MainWindowHandle { get => process.MainWindowHandle; }

    public bool HasExited
    {
        get
        {
            try
            {
                return process.HasExited;
            }
            catch (InvalidOperationException)
            {
                // No process is associated with this object.
                return true;
            }
            catch (Win32Exception win32Ex)
            {
                if ((WIN32_ERROR)win32Ex.NativeErrorCode == WIN32_ERROR.ERROR_ACCESS_DENIED)
                {
                    return false;
                }

                throw;
            }
        }
    }

    public int ExitCode { get => process.ExitCode; }

    public void Start()
    {
        process.Start();
    }

    public void ResumeMainThread()
    {
        HutaoException.NotSupported("ResumeMainThread is not supported for System.Diagnostics.Process.");
    }

    public void WaitForExit()
    {
        process.WaitForExit();
    }

    public void Kill()
    {
        process.Kill();
    }

    public void Dispose()
    {
        process.Dispose();
    }
}