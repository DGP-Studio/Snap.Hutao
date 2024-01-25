// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows5.1.2600")]
internal unsafe struct IModalWindow
{
    internal static Guid IID = new(3034256983u, 28887, 18526, 142, 62, 111, 203, 90, 92, 24, 2);

    private readonly Vftbl* thisPtr;

    internal unsafe readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IModalWindow*, HWND, HRESULT> Show;
    }
}