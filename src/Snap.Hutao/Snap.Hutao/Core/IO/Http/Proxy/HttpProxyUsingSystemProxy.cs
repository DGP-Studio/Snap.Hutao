// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32.Registry;
using System.Net;
using System.Reflection;

namespace Snap.Hutao.Core.IO.Http.Proxy;

[Injection(InjectAs.Singleton)]
internal sealed partial class HttpProxyUsingSystemProxy : ObservableObject, IWebProxy, IDisposable
{
    private const string ProxySettingPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections";

    private static readonly Lazy<MethodInfo> LazyConstructSystemProxyMethod = new(GetConstructSystemProxyMethod);

    private readonly IServiceProvider serviceProvider;
    private readonly RegistryWatcher watcher;

    private IWebProxy innerProxy = default!;

    public HttpProxyUsingSystemProxy(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        UpdateInnerProxy();

        watcher = new(ProxySettingPath, OnSystemProxySettingsChanged);
        watcher.Start();
    }

    public string CurrentProxyUri
    {
        get
        {
            Uri? proxyUri = GetProxy("https://hut.ao".ToUri());
            return proxyUri is null
                ? SH.ViewPageFeedbackCurrentProxyNoProxyDescription
                : proxyUri.AbsoluteUri;
        }
    }

    public IWebProxy InnerProxy
    {
        get => innerProxy;

        [MemberNotNull(nameof(innerProxy))]
        set
        {
            if (ReferenceEquals(innerProxy, value))
            {
                return;
            }

            (innerProxy as IDisposable)?.Dispose();
            innerProxy = value;
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
        (innerProxy as IDisposable)?.Dispose();
        watcher.Dispose();
    }

    public void OnSystemProxySettingsChanged()
    {
        UpdateInnerProxy();

        // TaskContext can't be injected directly since there are some recursive dependencies.
        ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        taskContext.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(CurrentProxyUri)));
    }

    private static MethodInfo GetConstructSystemProxyMethod()
    {
        Type? systemProxyInfoType = typeof(System.Net.Http.SocketsHttpHandler).Assembly.GetType("System.Net.Http.SystemProxyInfo");
        ArgumentNullException.ThrowIfNull(systemProxyInfoType);

        MethodInfo? constructSystemProxyMethod = systemProxyInfoType.GetMethod("ConstructSystemProxy", BindingFlags.Static | BindingFlags.Public);
        ArgumentNullException.ThrowIfNull(constructSystemProxyMethod);

        return constructSystemProxyMethod;
    }

    [MemberNotNull(nameof(innerProxy))]
    private void UpdateInnerProxy()
    {
        IWebProxy? proxy = LazyConstructSystemProxyMethod.Value.Invoke(default, default) as IWebProxy;
        ArgumentNullException.ThrowIfNull(proxy);

        InnerProxy = proxy;
    }
}