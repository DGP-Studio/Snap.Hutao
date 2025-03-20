// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32.Registry;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.IO.Http.Proxy;

[SuppressMessage("", "CA1001")]
internal sealed partial class HttpProxyUsingSystemProxy : ObservableObject, IWebProxy
{
    private const string ProxySettingPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections";

    private static readonly Lazy<MethodInfo> LazyConstructSystemProxyMethod = new(GetConstructSystemProxyMethod);
    private static readonly Uri ProxyTestDestination = "https://hut.ao".ToUri();
    private static readonly Lock SyncRoot = new();

    private HttpProxyUsingSystemProxy()
    {
        UpdateInnerProxy();

        RegistryWatcher watcher = new(ProxySettingPath, OnSystemProxySettingsChanged);
        watcher.Start();
        GCHandle.Alloc(watcher);
    }

    [field: MaybeNull]
    public static HttpProxyUsingSystemProxy Instance
    {
        get
        {
            if (field is null)
            {
                lock (SyncRoot)
                {
                    field ??= new();
                }
            }

            return field;
        }
    }

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

    private static MethodInfo GetConstructSystemProxyMethod()
    {
        Type? systemProxyInfoType = typeof(System.Net.Http.SocketsHttpHandler).Assembly.GetType("System.Net.Http.SystemProxyInfo");
        ArgumentNullException.ThrowIfNull(systemProxyInfoType);

        MethodInfo? constructSystemProxyMethod = systemProxyInfoType.GetMethod("ConstructSystemProxy", BindingFlags.Static | BindingFlags.Public);
        ArgumentNullException.ThrowIfNull(constructSystemProxyMethod);

        return constructSystemProxyMethod;
    }

    [MemberNotNull(nameof(InnerProxy))]
    private void UpdateInnerProxy()
    {
        IWebProxy? proxy = LazyConstructSystemProxyMethod.Value.Invoke(default, default) as IWebProxy;
        ArgumentNullException.ThrowIfNull(proxy);

        InnerProxy = proxy;
    }

    private void OnSystemProxySettingsChanged()
    {
        UpdateInnerProxy();

        Debug.Assert(XamlApplicationLifetime.DispatcherQueueInitialized, "DispatcherQueue not initialized");
        SynchronizationContext.Current?.Post(_ => OnPropertyChanged(nameof(CurrentProxyUri)), default);
    }
}