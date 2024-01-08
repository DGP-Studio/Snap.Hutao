// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Shell;
using System.Net;

namespace Snap.Hutao.Core.IO.Http.DynamicProxy;

[Injection(InjectAs.Singleton)]
internal sealed partial class DynamicHttpProxy : IWebProxy, IDisposable
{
    private readonly RegistryMonitor registryMonitor;

    private IWebProxy innerProxy;

    public DynamicHttpProxy()
    {
        HttpWindowsProxy.TryCreate(out IWebProxy? proxy);
        innerProxy = proxy ?? new HttpNoProxy();

        registryMonitor = RegistryMonitor.Create(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections", OnRegistryChanged);
        registryMonitor.Start();
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
        HttpWindowsProxy.TryCreate(out IWebProxy? proxy);
        InnerProxy = proxy ?? new HttpNoProxy();
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

        registryMonitor.Dispose();
    }

    private void OnRegistryChanged(object? sender, EventArgs e)
    {
        UpdateProxy();
    }
}