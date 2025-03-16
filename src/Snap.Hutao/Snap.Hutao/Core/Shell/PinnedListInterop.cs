// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.UI.Shell;
using Snap.Hutao.Win32.UI.Shell.Common;
using System.Runtime.CompilerServices;
using WinRT;
using static Snap.Hutao.Win32.Macros;
using static Snap.Hutao.Win32.Shell32;
using static Snap.Hutao.Win32.Ole32;

namespace Snap.Hutao.Core.Shell;

[Injection(InjectAs.Transient, typeof(IPinnedListInterop))]
internal sealed class PinnedListInterop : IPinnedListInterop
{
    public bool TryPinShortcut(string shortcutPath)
    {
        if (!SUCCEEDED(CoCreateInstance(in TaskbandPin.CLSID, default, CLSCTX.CLSCTX_ALL, in IPinnedList3.IID, out ObjectReference<IPinnedList3.Vftbl> pinnedList)))
        {
            return false;
        }

        using (pinnedList)
        {
            ref ITEMIDLIST list = ref ILCreateFromPathW(shortcutPath);
            try
            {
                return SUCCEEDED(pinnedList.Modify(ref Unsafe.NullRef<ITEMIDLIST>(), ref list, PINNEDLISTMODIFYCALLER.PMC_UNIFIEDTILEMODELBROKER));
            }
            finally
            {
                ILFree(ref list);
            }
        }
    }

    public bool TryUnpinShortcut(string shortcutPath)
    {
        if (!SUCCEEDED(CoCreateInstance(in TaskbandPin.CLSID, default, CLSCTX.CLSCTX_ALL, in IPinnedList3.IID, out ObjectReference<IPinnedList3.Vftbl> pinnedList)))
        {
            return false;
        }

        using (pinnedList)
        {
            ref ITEMIDLIST list = ref ILCreateFromPathW(shortcutPath);
            try
            {
                return SUCCEEDED(pinnedList.Modify(ref list, ref Unsafe.NullRef<ITEMIDLIST>(), PINNEDLISTMODIFYCALLER.PMC_UNIFIEDTILEMODELBROKER));
            }
            finally
            {
                ILFree(ref list);
            }
        }
    }
}