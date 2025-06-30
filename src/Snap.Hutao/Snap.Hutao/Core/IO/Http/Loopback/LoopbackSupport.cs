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
            if (!native.IsPublicFirewallEnabled())
            {
                IsLoopbackEnabled = false;
                hutaoContainerStringSid = string.Empty;
                return;
            }

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
    [NotifyPropertyChangedFor(nameof(CanEnableLoopback))]
    public partial bool IsLoopbackEnabled { get; private set; }

    public bool CanEnableLoopback
    {
        get => HutaoRuntime.IsProcessElevated && !IsLoopbackEnabled && !string.IsNullOrEmpty(hutaoContainerStringSid);
    }

    public void EnableLoopback()
    {
        if (!CanEnableLoopback)
        {
            return;
        }

        native.Enable(hutaoContainerStringSid);
        IsLoopbackEnabled = true;
    }
}