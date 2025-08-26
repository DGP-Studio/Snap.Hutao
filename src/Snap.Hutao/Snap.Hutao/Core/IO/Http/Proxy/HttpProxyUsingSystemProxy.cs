// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.IO.Http.Proxy;

[SuppressMessage("", "CA1001")]
internal sealed partial class HttpProxyUsingSystemProxy : ObservableObject, IWebProxy
{
    private const string ProxySettingPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections";

    private static readonly Lazy<MethodInfo> LazyConstructSystemProxyMethod = new(GetConstructSystemProxyMethod);
    private static readonly Uri ProxyTestDestination = "https://hut.ao".ToUri();

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly HutaoNativeRegistryNotification native;

    private unsafe HttpProxyUsingSystemProxy()
    {
        UpdateInnerProxy();

        native = HutaoNative.Instance.MakeRegistryNotification(ProxySettingPath);
        native.Start(HutaoNativeRegistryNotificationCallback.Create(&OnSystemProxySettingsChanged), 0);
    }

    [field: MaybeNull]
    public static HttpProxyUsingSystemProxy Instance { get => LazyInitializer.EnsureInitialized(ref field); }

    [SuppressMessage("", "SA1201")]
    public string CurrentProxyUri { get => GetProxy(ProxyTestDestination)?.AbsoluteUri ?? SH.ViewPageFeedbackCurrentProxyNoProxyDescription; }

    public IWebProxy InnerProxy
    {
        get;
        set
        {
            if (ReferenceEquals(field, value))
            {
                return;
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            (field as IDisposable)?.Dispose();
            field = value;
        }
    }

    public ICredentials? Credentials
    {
        get => InnerProxy.Credentials;
        set => InnerProxy.Credentials = value;
    }

    public Uri? GetProxy(Uri destination)
    {
        return InnerProxy.GetProxy(destination);
    }

    public bool IsBypassed(Uri host)
    {
        return InnerProxy.IsBypassed(host);
    }

#if NET10_0_OR_GREATER
    [Obsolete("Use UnsafeAccessor")]
#endif
    private static MethodInfo GetConstructSystemProxyMethod()
    {
        Type? systemProxyInfoType = typeof(System.Net.Http.SocketsHttpHandler).Assembly.GetType("System.Net.Http.SystemProxyInfo");
        ArgumentNullException.ThrowIfNull(systemProxyInfoType);

        MethodInfo? constructSystemProxyMethod = systemProxyInfoType.GetMethod("ConstructSystemProxy", BindingFlags.Static | BindingFlags.Public);
        ArgumentNullException.ThrowIfNull(constructSystemProxyMethod);

        return constructSystemProxyMethod;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void OnSystemProxySettingsChanged(nint userData)
    {
        if (XamlApplicationLifetime.Exiting)
        {
            return;
        }

        Instance.UpdateInnerProxy();

        Debug.Assert(XamlApplicationLifetime.DispatcherQueueInitialized, "DispatcherQueue not initialized");
        SynchronizationContext.Current?.Post(static _ => Instance.OnPropertyChanged(nameof(CurrentProxyUri)), default);
    }

    [MemberNotNull(nameof(InnerProxy))]
    private void UpdateInnerProxy()
    {
        IWebProxy? proxy = LazyConstructSystemProxyMethod.Value.Invoke(default, BindingFlags.DoNotWrapExceptions, default, default, default) as IWebProxy;
        ArgumentNullException.ThrowIfNull(proxy);
        InnerProxy = proxy;
    }
}