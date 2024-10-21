// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpRequestMessageExtension
{
    private const int MessageNotYetSent = 0;

    public static void Resurrect(this HttpRequestMessage httpRequestMessage)
    {
        Interlocked.Exchange(ref GetPrivateSendStatus(httpRequestMessage), MessageNotYetSent);

        if (httpRequestMessage.Content is { } content)
        {
            Volatile.Write(ref GetPrivatedBufferedContent(content), default);
            Volatile.Write(ref GetPrivatedDisposed(content), false);
        }
    }

    // private int _sendStatus
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_sendStatus")]
    private static extern ref int GetPrivateSendStatus(HttpRequestMessage message);

    // private bool _disposed
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_disposed")]
    private static extern ref bool GetPrivatedDisposed(HttpContent content);

    // private MemoryStream? _bufferedContent
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_bufferedContent")]
    private static extern ref MemoryStream? GetPrivatedBufferedContent(HttpContent content);
}