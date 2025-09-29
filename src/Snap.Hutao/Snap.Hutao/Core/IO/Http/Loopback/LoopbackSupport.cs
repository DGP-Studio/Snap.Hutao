// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32;

namespace Snap.Hutao.Core.IO.Http.Loopback;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class LoopbackSupport : ObservableObject
{
    private readonly HutaoNativeLoopbackSupport native;
    private readonly string hutaoContainerStringSid;

    public LoopbackSupport(IServiceProvider serviceProvider)
    {
        native = HutaoNative.Instance.MakeLoopbackSupport();
        try
        {
            if (!native.IsPublicFirewallEnabled)
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

#pragma warning disable SA1116, SA1117
        SentrySdk.ConfigureScope(static (scope, state) =>
        {
            Dictionary<string, object> loopback = new()
            {
                ["Enabled"] = state.Enabled,
                ["Sid"] = state.Sid,
            };

            scope.Contexts["Loopback"] = loopback;
        }, (Sid: hutaoContainerStringSid, Enabled: IsLoopbackEnabled));
#pragma warning restore SA1116, SA1117
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