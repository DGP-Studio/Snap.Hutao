#pragma warning disable
namespace Snap.Hutao.Core.IO.Http.DynamicProxy;

/// <summary>
/// Copied from https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/Common/src/System/Net/UriScheme.cs
/// </summary>
internal static class UriScheme
{
    public const string File = "file";
    public const string Ftp = "ftp";
    public const string Gopher = "gopher";
    public const string Http = "http";
    public const string Https = "https";
    public const string News = "news";
    public const string NetPipe = "net.pipe";
    public const string NetTcp = "net.tcp";
    public const string Nntp = "nntp";
    public const string Mailto = "mailto";
    public const string Ws = "ws";
    public const string Wss = "wss";

    public const string SchemeDelimiter = "://";
}
#pragma warning restore