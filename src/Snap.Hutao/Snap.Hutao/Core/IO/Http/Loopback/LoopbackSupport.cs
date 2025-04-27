// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Core.IO.Http.Loopback;

[Injection(InjectAs.Singleton)]
internal sealed partial class LoopbackSupport : ObservableObject
{
    private readonly ObjectReference<HutaoNative.IHutaoNativeLoopbackSupport> native;
    private readonly string hutaoContainerStringSid;

    public LoopbackSupport(IServiceProvider serviceProvider)
    {
        Marshal.ThrowExceptionForHR(HutaoNative.Instance.MakeLoopbackSupport(out native));
        Marshal.ThrowExceptionForHR(native.IsEnabled(HutaoRuntime.FamilyName, out ReadOnlySpan<char> sid, out BOOL enabled));
        hutaoContainerStringSid = sid.ToString();
        IsLoopbackEnabled = enabled;
    }

    [ObservableProperty]
    public partial bool IsLoopbackEnabled { get; private set; }

    public void EnableLoopback()
    {
        Marshal.ThrowExceptionForHR(native.Enable(hutaoContainerStringSid));
        IsLoopbackEnabled = true;
    }
}