#pragma warning disable
using System.Net;

namespace Snap.Hutao.Core.IO.Http.DynamicProxy;

/// <summary>
/// Copied from https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/HttpNoProxy.cs
/// </summary>
internal sealed class HttpNoProxy : IWebProxy
{
    public ICredentials? Credentials { get; set; }
    public Uri? GetProxy(Uri destination) => null;
    public bool IsBypassed(Uri host) => true;
}
#pragma warning restore