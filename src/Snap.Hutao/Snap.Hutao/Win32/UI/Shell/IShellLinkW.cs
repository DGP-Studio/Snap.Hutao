// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Storage.FileSystem;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell.Common;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows5.1.2600")]
[Guid("000214F9-0000-0000-C000-000000000046")]
internal unsafe struct IShellLinkW
{
    public readonly Vftbl* ThisPtr;

    internal static unsafe ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xF9, 0x14, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public unsafe HRESULT QueryInterface<TInterface>(ref readonly Guid riid, out TInterface* pvObject)
        where TInterface : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (TInterface** ppvObject = &pvObject)
            {
                return ThisPtr->IUnknownVftbl.QueryInterface((IUnknown*)Unsafe.AsPointer(ref this), riid2, (void**)ppvObject);
            }
        }
    }

    public uint AddRef()
    {
        return ThisPtr->IUnknownVftbl.AddRef((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public uint Release()
    {
        return ThisPtr->IUnknownVftbl.Release((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public HRESULT SetShowCmd(SHOW_WINDOW_CMD iShowCmd)
    {
        return ThisPtr->SetShowCmd((IShellLinkW*)Unsafe.AsPointer(ref this), iShowCmd);
    }

    public HRESULT SetIconLocation(string szIconPath, int iIcon)
    {
        fixed (char* pszIconPath = szIconPath)
        {
            return ThisPtr->SetIconLocation((IShellLinkW*)Unsafe.AsPointer(ref this), pszIconPath, iIcon);
        }
    }

    public unsafe HRESULT SetPath(string szFile)
    {
        fixed (char* pszFile = szFile)
        {
            return ThisPtr->SetPath((IShellLinkW*)Unsafe.AsPointer(ref this), pszFile);
        }
    }

    internal unsafe readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, int, WIN32_FIND_DATAW*, uint, HRESULT> GetPath;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, ITEMIDLIST**, HRESULT> GetIDList;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, ITEMIDLIST*, HRESULT> SetIDList;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, int, HRESULT> GetDescription;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, HRESULT> SetDescription;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, int, HRESULT> GetWorkingDirectory;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, HRESULT> SetWorkingDirectory;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, int, HRESULT> GetArguments;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, HRESULT> SetArguments;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, ushort*, HRESULT> GetHotkey;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, ushort, HRESULT> SetHotkey;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, SHOW_WINDOW_CMD*, HRESULT> GetShowCmd;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, SHOW_WINDOW_CMD, HRESULT> SetShowCmd;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, int, int*, HRESULT> GetIconLocation;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, int, HRESULT> SetIconLocation;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, uint, HRESULT> SetRelativePath;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, HWND, uint, HRESULT> Resolve;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkW*, PWSTR, HRESULT> SetPath;
    }
}