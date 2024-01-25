// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal unsafe struct IShellItemFilter
{
    internal static Guid IID = new(643413109u, 61112, 18615, 143, 7, 179, 120, 129, 15, 72, 207);

    private Vftbl* thisPtr;

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IShellItemFilter*, IShellItem*, HRESULT> IncludeItem;
        internal readonly delegate* unmanaged[Stdcall]<IShellItemFilter*, IShellItem*, uint*, HRESULT> GetEnumFlagsForItem;
    }
}