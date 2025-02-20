// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell;
using Snap.Hutao.Win32.UI.Shell.Common;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SYSLIB1054")]
internal static class Shell32
{
    [DllImport("SHELL32.dll", CallingConvention = CallingConvention.Winapi,ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern unsafe ITEMIDLIST* ILCreateFromPathW(PCWSTR pszPath);

    public static unsafe ref ITEMIDLIST ILCreateFromPathW(ReadOnlySpan<char> szPath)
    {
        fixed (char* pszPath = szPath)
        {
            return ref *ILCreateFromPathW(pszPath);
        }
    }

    [DllImport("SHELL32.dll", ExactSpelling = true, PreserveSig = false)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern unsafe void ILFree(ITEMIDLIST* pidl);

    public static unsafe void ILFree(ref ITEMIDLIST idl)
    {
        fixed(ITEMIDLIST* pidl = &idl)
        {
            ILFree(pidl);
        }
    }

    [DllImport("SHELL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows6.0.6000")]
    public static extern unsafe HRESULT SHCreateItemFromParsingName(PCWSTR pszPath, [Optional] nint pbc, Guid* riid, void** ppv);

    public static unsafe HRESULT SHCreateItemFromParsingName<TVftbl>(ReadOnlySpan<char> szPath, [Optional] ObjectReference<IBindCtx.Vftbl>? bc, ref readonly Guid iid, out ObjectReference<TVftbl> v)
        where TVftbl : unmanaged
    {
        fixed (char* pszPath = szPath)
        {
            fixed (Guid* riid = &iid)
            {
                nint pv = default;
                HRESULT hr = SHCreateItemFromParsingName(pszPath, MarshalInterfaceHelper<object>.GetAbi(bc), riid, (void**)&pv);
                v = ObjectReference<TVftbl>.Attach(ref pv, iid);
                return hr;
            }
        }
    }

    [DllImport("SHELL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows6.1")]
    public static extern unsafe HRESULT Shell_NotifyIconGetRect(NOTIFYICONIDENTIFIER* identifier, RECT* iconLocation);

    public static unsafe HRESULT Shell_NotifyIconGetRect(ref readonly NOTIFYICONIDENTIFIER identifier, out RECT iconLocation)
    {
        fixed (NOTIFYICONIDENTIFIER* p = &identifier)
        {
            fixed (RECT* pRect = &iconLocation)
            {
                return Shell_NotifyIconGetRect(p, pRect);
            }
        }
    }

    [DllImport("SHELL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern unsafe BOOL Shell_NotifyIconW(NOTIFY_ICON_MESSAGE dwMessage, NOTIFYICONDATAW* lpData);

    public static unsafe BOOL Shell_NotifyIconW(NOTIFY_ICON_MESSAGE dwMessage, ref readonly NOTIFYICONDATAW data)
    {
        fixed (NOTIFYICONDATAW* p = &data)
        {
            return Shell_NotifyIconW(dwMessage, p);
        }
    }
}