// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SH002")]
[SuppressMessage("", "SYSLIB1054")]
internal static class Shell32
{
    [DllImport("SHELL32.dll", ExactSpelling = true)]
    [SupportedOSPlatform("windows6.0.6000")]
    public static unsafe extern HRESULT SHCreateItemFromParsingName(PCWSTR pszPath, [AllowNull] IBindCtx pbc, Guid* riid, void** ppv);

    [DebuggerStepThrough]
    public static unsafe HRESULT SHCreateItemFromParsingName<T>(ReadOnlySpan<char> szPath, [AllowNull] IBindCtx pbc, ref readonly Guid riid, out T* pv)
        where T : unmanaged
    {
        fixed (char* pszPath = szPath)
        {
            fixed (Guid* pGuid = &riid)
            {
                fixed (T** ppv = &pv)
                {
                    return SHCreateItemFromParsingName(pszPath, pbc, pGuid, (void**)ppv);
                }
            }
        }
    }
}