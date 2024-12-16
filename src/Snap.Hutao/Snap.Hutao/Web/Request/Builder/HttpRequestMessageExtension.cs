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
        // Mark the message as not yet sent
        Interlocked.Exchange(ref GetPrivateSendStatus(httpRequestMessage), MessageNotYetSent);

        if (httpRequestMessage.Content is { } content)
        {
            // Clear the buffered content, so that it can trigger a new read attempt
            Volatile.Write(ref GetPrivateBufferedContent(content), default);
            Volatile.Write(ref GetPrivateDisposed(content), false);
        }
    }

    // private int _sendStatus
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_sendStatus")]
    private static extern ref int GetPrivateSendStatus(HttpRequestMessage message);

    // private bool _disposed
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_disposed")]
    private static extern ref bool GetPrivateDisposed(HttpContent content);

    // private MemoryStream? _bufferedContent
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_bufferedContent")]
    private static extern ref MemoryStream? GetPrivateBufferedContent(HttpContent content);
}