// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32.Registry;
using System.Diagnostics;
using System.Net;
using System.Reflection;

namespace Snap.Hutao.Core.IO.Http.Proxy;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class HttpProxyUsingSystemProxy : ObservableObject, IWebProxy, IDisposable
{
    private const string ProxySettingPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections";

    private static readonly Lazy<MethodInfo> LazyConstructSystemProxyMethod = new(GetConstructSystemProxyMethod);
    private static readonly Uri ProxyTestDestination = "https://hut.ao".ToUri();

    private RegistryWatcher watcher;

    partial void PostConstruct(IServiceProvider serviceProvider)
    {
        UpdateInnerProxy();

        watcher = new(ProxySettingPath, OnSystemProxySettingsChanged);
        watcher.Start(serviceProvider.GetRequiredService<ILogger<HttpProxyUsingSystemProxy>>());
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

    public void Dispose()
    {
        watcher.Dispose();
        (InnerProxy as IDisposable)?.Dispose();
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