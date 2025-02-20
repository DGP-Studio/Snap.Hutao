// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell.Common;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows10.0.17763.0")]
internal static unsafe class IPinnedList3
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xE2, 0x9A, 0xD7, 0x0D, 0x56, 0xD1, 0xD4, 0x45, 0x9E, 0xEB, 0x3B, 0x54, 0x97, 0x69, 0xE9, 0x40]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> EnumObjects;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, int, nint*, nint*, PWSTR*, int*, HRESULT> GetPinnableInfo;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> IsPinnable;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, ulong, ITEMIDLIST*, ITEMIDLIST**, HRESULT> Resovle;
        internal readonly delegate* unmanaged[Stdcall]<nint, ulong, HRESULT> Unadvise;
        internal readonly delegate* unmanaged[Stdcall]<nint, ulong*, HRESULT> GetChangeCount;
        internal readonly delegate* unmanaged[Stdcall]<nint, ITEMIDLIST*, HRESULT> IsPinned;
        internal readonly delegate* unmanaged[Stdcall]<nint, ITEMIDLIST*, ITEMIDLIST**, HRESULT> GetPinnedItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, ITEMIDLIST*, PWSTR*, HRESULT> GetAppIDForPinnedItem;
        internal readonly delegate* unmanaged[Stdcall]<nint, ITEMIDLIST*, ITEMIDLIST*, HRESULT> ItemChangeNotify;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> UpdateForRemovedItemsAsNecessary;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, nint, HRESULT> PinShellLink;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, ITEMIDLIST*, HRESULT> GetPinnedItemForAppID;
        internal readonly delegate* unmanaged[Stdcall]<nint, ITEMIDLIST*, ITEMIDLIST*, PINNEDLISTMODIFYCALLER, HRESULT> Modify;
    }
}