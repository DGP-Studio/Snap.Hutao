// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Shell;
using System.Net;
using System.Reflection;

namespace Snap.Hutao.Core.IO.Http.DynamicProxy;

[Injection(InjectAs.Singleton)]
internal sealed partial class DynamicHttpProxy : IWebProxy, IDisposable
{
    private readonly RegistryWatcher watcher;

    private IWebProxy innerProxy = default!;

    public DynamicHttpProxy()
    {
        UpdateProxy();

        watcher = new (@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections");
        watcher.Start();
    }

    /// <inheritdoc/>
    public ICredentials? Credentials
    {
        get => InnerProxy.Credentials;
        set => InnerProxy.Credentials = value;
    }

    private IWebProxy InnerProxy
    {
        get => innerProxy;
        set
        {
            if (ReferenceEquals(innerProxy, value))
            {
                return;
            }

            if (innerProxy is IDisposable disposable)
            {
                disposable.Dispose();
            }

            innerProxy = value;
        }
    }

    public void UpdateProxy()
    {
        Assembly httpNamespace = Assembly.Load("System.Net.Http");
        Type? systemProxyInfoType = httpNamespace.GetType("System.Net.Http.SystemProxyInfo");
        ArgumentNullException.ThrowIfNull(systemProxyInfoType);
        MethodInfo? constructSystemProxyMethod = systemProxyInfoType.GetMethod("ConstructSystemProxy", BindingFlags.Static | BindingFlags.Public);
        ArgumentNullException.ThrowIfNull(constructSystemProxyMethod);
        IWebProxy? proxy = (IWebProxy?)constructSystemProxyMethod.Invoke(null, null);
        ArgumentNullException.ThrowIfNull(proxy);
        InnerProxy = proxy;
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
        if (innerProxy is IDisposable disposable)
        {
            disposable.Dispose();
        }

        watcher.Dispose();
    }

    private void OnRegistryChanged(object? sender, EventArgs e)
    {
        UpdateProxy();
    }
}