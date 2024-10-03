// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Storage.FileSystem;
using Snap.Hutao.Win32.UI.Shell.Common;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows5.1.2600")]
internal static unsafe class IShellLinkW
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xF9, 0x14, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    public static HRESULT SetArguments(this ObjectReference<Vftbl> objRef, ReadOnlySpan<char> szArgs)
    {
        fixed (char* pszArgs = szArgs)
        {
            return objRef.Vftbl.SetArguments(objRef.ThisPtr, pszArgs);
        }
    }

    public static HRESULT SetShowCmd(this ObjectReference<Vftbl> objRef, SHOW_WINDOW_CMD iShowCmd)
    {
        return objRef.Vftbl.SetShowCmd(objRef.ThisPtr, iShowCmd);
    }

    public static HRESULT SetIconLocation(this ObjectReference<Vftbl> objRef, ReadOnlySpan<char> szIconPath, int iIcon)
    {
        fixed (char* pszIconPath = szIconPath)
        {
            return objRef.Vftbl.SetIconLocation(objRef.ThisPtr, pszIconPath, iIcon);
        }
    }

    public static HRESULT SetPath(this ObjectReference<Vftbl> objRef, ReadOnlySpan<char> szFile)
    {
        fixed (char* pszFile = szFile)
        {
            return objRef.Vftbl.SetPath(objRef.ThisPtr, pszFile);
        }
    }

    internal readonly unsafe struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, int, WIN32_FIND_DATAW*, uint, HRESULT> GetPath;
        internal readonly delegate* unmanaged[Stdcall]<nint, ITEMIDLIST**, HRESULT> GetIDList;
        internal readonly delegate* unmanaged[Stdcall]<nint, ITEMIDLIST*, HRESULT> SetIDList;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, int, HRESULT> GetDescription;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, HRESULT> SetDescription;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, int, HRESULT> GetWorkingDirectory;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, HRESULT> SetWorkingDirectory;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, int, HRESULT> GetArguments;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, HRESULT> SetArguments;
        internal readonly delegate* unmanaged[Stdcall]<nint, ushort*, HRESULT> GetHotkey;
        internal readonly delegate* unmanaged[Stdcall]<nint, ushort, HRESULT> SetHotkey;
        internal readonly delegate* unmanaged[Stdcall]<nint, SHOW_WINDOW_CMD*, HRESULT> GetShowCmd;
        internal readonly delegate* unmanaged[Stdcall]<nint, SHOW_WINDOW_CMD, HRESULT> SetShowCmd;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, int, int*, HRESULT> GetIconLocation;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, int, HRESULT> SetIconLocation;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, uint, HRESULT> SetRelativePath;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, uint, HRESULT> Resolve;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, HRESULT> SetPath;
    }
}