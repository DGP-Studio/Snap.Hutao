// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.IO.Http.Loopback;
using Snap.Hutao.Win32.Registry;
using System.Net;
using System.Reflection;

namespace Snap.Hutao.Core.IO.Http.Proxy;

[Injection(InjectAs.Singleton)]
internal sealed partial class HttpProxyUsingSystemProxy : ObservableObject, IWebProxy, IDisposable
{
    private const string ProxySettingPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections";

    private static readonly Lazy<MethodInfo> LazyConstructSystemProxyMethod = new(GetConstructSystemProxyMethod);
    private static readonly Uri ProxyTestDestination = "https://hut.ao".ToUri();

    private readonly IServiceProvider serviceProvider;
    private readonly LoopbackSupport loopbackSupport;
    private readonly RegistryWatcher watcher;

    public HttpProxyUsingSystemProxy(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        loopbackSupport = serviceProvider.GetRequiredService<LoopbackSupport>();
        UpdateInnerProxy();

        watcher = new(ProxySettingPath, OnSystemProxySettingsChanged);
        watcher.Start(serviceProvider.GetRequiredService<ILogger<HttpProxyUsingSystemProxy>>());
    }

    public bool IsProxyWorking
    {
        get => GetProxy(ProxyTestDestination) is not null && loopbackSupport.IsLoopbackEnabled;
    }

    public string CurrentProxyUri
    {
        get
        {
            Uri? proxyUri = GetProxy(ProxyTestDestination);
            return proxyUri is null
                ? SH.ViewPageFeedbackCurrentProxyNoProxyDescription
                : proxyUri.AbsoluteUri;
        }
    }

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

    public void OnSystemProxySettingsChanged()
    {
        UpdateInnerProxy();

        // TaskContext can't be injected directly since there are some recursive dependencies.
        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        taskContext.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(CurrentProxyUri)));
    }

    [Command("EnableLoopbackCommand")]
    public void EnableLoopback()
    {
        loopbackSupport.EnableLoopback();
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
}