#pragma warning disable
using System.Net;

namespace Snap.Hutao.Core.IO.Http.DynamicProxy;

/// <summary>
/// Copied from https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/IMultiWebProxy.cs
/// </summary>
internal interface IMultiWebProxy : IWebProxy
{
    /// <summary>
    /// Gets the proxy URIs.
    /// </summary>
    public MultiProxy GetMultiProxy(Uri uri);
}
#pragma warning restore