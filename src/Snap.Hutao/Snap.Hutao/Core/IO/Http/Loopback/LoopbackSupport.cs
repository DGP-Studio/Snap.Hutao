// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32;

namespace Snap.Hutao.Core.IO.Http.Loopback;

[Injection(InjectAs.Singleton)]
internal sealed partial class LoopbackSupport : ObservableObject
{
    private readonly HutaoNativeLoopbackSupport native;
    private readonly string hutaoContainerStringSid;

    public LoopbackSupport(IServiceProvider serviceProvider)
    {
        native = HutaoNative.Instance.MakeLoopbackSupport();
        try
        {
            IsLoopbackEnabled = native.IsEnabled(HutaoRuntime.FamilyName, out string? sid);
            hutaoContainerStringSid = sid ?? string.Empty;
        }
        catch
        {
            IsLoopbackEnabled = false;
            hutaoContainerStringSid = string.Empty;
        }
    }

    [ObservableProperty]
    public partial bool IsLoopbackEnabled { get; private set; }

    public void EnableLoopback()
    {
        native.Enable(hutaoContainerStringSid);
        IsLoopbackEnabled = true;
    }
}