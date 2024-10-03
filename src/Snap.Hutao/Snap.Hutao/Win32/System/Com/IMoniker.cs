// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal static unsafe class IMoniker
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IPersistStream.Vftbl IPersistStreamVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, Guid*, void**, HRESULT> BindToObject;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, Guid*, void**, HRESULT> BindToStorage;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, nint*, nint*, HRESULT> Reduce;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, BOOL, nint*, HRESULT> ComposeWith;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL, nint*, HRESULT> Enum;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> IsEqual;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint*, HRESULT> Hash;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, nint, HRESULT> IsRunning;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, FILETIME*, HRESULT> GetTimeOfLastChange;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> Inverse;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint*, HRESULT> CommonPrefixWith;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint*, HRESULT> RelativePathTo;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, PWSTR*, HRESULT> GetDisplayName;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, PWSTR, uint*, nint*, HRESULT> ParseDisplayName;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint*, HRESULT> IsSystemMoniker;
    }
}
