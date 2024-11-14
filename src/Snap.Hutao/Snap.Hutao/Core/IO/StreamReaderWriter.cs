// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

internal sealed partial class StreamReaderWriter : IDisposable
{
    public StreamReaderWriter(StreamReader reader, StreamWriter writer)
    {
        Reader = reader;
        Writer = writer;
    }

    public StreamReader Reader { get; }

    public StreamWriter Writer { get; }

    /// <inheritdoc cref="StreamReader.ReadLineAsync(CancellationToken)"/>
    public ValueTask<string?> ReadLineAsync(CancellationToken token)
    {
        return Reader.ReadLineAsync(token);
    }

    /// <inheritdoc cref="StreamWriter.WriteAsync(string?)"/>
    [SuppressMessage("", "SH003")]
    public Task WriteAsync(string value)
    {
        return Writer.WriteAsync(value);
    }

    public void Dispose()
    {
        Writer.Dispose();
        Reader.Dispose();
    }
}