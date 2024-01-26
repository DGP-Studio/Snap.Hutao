// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Web;
using Snap.Hutao.Win32.Registry;
using System.Net;
using System.Reflection;

namespace Snap.Hutao.Core.IO.Http.DynamicProxy;

[Injection(InjectAs.Singleton)]
internal sealed partial class DynamicHttpProxy : ObservableObject, IWebProxy, IDisposable
{
    private const string ProxySettingPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections";

    private static readonly MethodInfo ConstructSystemProxyMethod;

    private readonly RegistryWatcher watcher;

    private IWebProxy innerProxy = default!;

    [SuppressMessage("", "CA1810")]
    static DynamicHttpProxy()
    {
        Type? systemProxyInfoType = typeof(System.Net.Http.SocketsHttpHandler).Assembly.GetType("System.Net.Http.SystemProxyInfo");
        ArgumentNullException.ThrowIfNull(systemProxyInfoType);

        MethodInfo? constructSystemProxyMethod = systemProxyInfoType.GetMethod("ConstructSystemProxy", BindingFlags.Static | BindingFlags.Public);
        ArgumentNullException.ThrowIfNull(constructSystemProxyMethod);
        ConstructSystemProxyMethod = constructSystemProxyMethod;
    }

    public DynamicHttpProxy()
    {
        UpdateProxy();

        watcher = new(ProxySettingPath, OnProxyChanged);
        watcher.Start();
    }

    /// <inheritdoc/>
    public ICredentials? Credentials
    {
        get => InnerProxy.Credentials;
        set => InnerProxy.Credentials = value;
    }

    public string CurrentProxy
    {
        get
        {
            Uri? proxyUri = GetProxy(HutaoEndpoints.Website(string.Empty).ToUri());
            return proxyUri is null
                ? SH.ViewPageFeedbackCurrentProxyNoProxyDescription
                : proxyUri.AbsoluteUri;
        }
    }

    private IWebProxy InnerProxy
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

    public void OnProxyChanged()
    {
        UpdateProxy();

        Ioc.Default.GetRequiredService<ITaskContext>().InvokeOnMainThread(() => OnPropertyChanged(nameof(CurrentProxy)));
    }

    [MemberNotNull(nameof(innerProxy))]
    private void UpdateProxy()
    {
        IWebProxy? proxy = ConstructSystemProxyMethod.Invoke(default, default) as IWebProxy;
        ArgumentNullException.ThrowIfNull(proxy);

        InnerProxy = proxy;
    }
}