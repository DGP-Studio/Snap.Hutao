// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeProcess
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeProcess(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public uint Id
    {
        get
        {
            uint id;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetId(objRef.ThisPtr, &id));
            return id;
        }
    }

    public HANDLE ProcessHandle
    {
        get
        {
            HANDLE hProcess;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetProcessHandle(objRef.ThisPtr, &hProcess));
            return hProcess;
        }
    }

    public HWND MainWindowHandle
    {
        get
        {
            HWND hWnd;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetMainWindowHandle(objRef.ThisPtr, &hWnd));
            return hWnd;
        }
    }

    public void Start()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Start(objRef.ThisPtr));
    }

    public void ResumeMainThread()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.ResumeMainThread(objRef.ThisPtr));
    }

    public void WaitForExit()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.WaitForExit(objRef.ThisPtr));
    }

    public void Kill()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Kill(objRef.ThisPtr));
    }

    public BOOL GetExitCodeProcess(out uint exitCode)
    {
        fixed (uint* pExitCode = &exitCode)
        {
            BOOL isRunning;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.GetExitCodeProcess(objRef.ThisPtr, &isRunning, pExitCode));
            return isRunning;
        }
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeProcess)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Start;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> ResumeMainThread;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> WaitForExit;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Kill;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint*, HRESULT> GetId;
        internal readonly delegate* unmanaged[Stdcall]<nint, HANDLE*, HRESULT> GetProcessHandle;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND*, HRESULT> GetMainWindowHandle;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, uint*, HRESULT> GetExitCodeProcess;
#pragma warning restore CS0649
    }
}