// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

internal sealed class StreamReaderWriter : IDisposable
{
    private readonly StreamReader reader;
    private readonly StreamWriter writer;

    public StreamReaderWriter(StreamReader reader, StreamWriter writer)
    {
        this.reader = reader;
        this.writer = writer;
    }

    public StreamReader Reader { get => reader; }

    public StreamWriter Writer { get => writer; }

    /// <inheritdoc cref="StreamReader.ReadLineAsync(CancellationToken)"/>
    public ValueTask<string?> ReadLineAsync(CancellationToken token)
    {
        return reader.ReadLineAsync(token);
    }

    /// <inheritdoc cref="StreamWriter.WriteAsync(string?)"/>
    [SuppressMessage("", "SH003")]
    public Task WriteAsync(string value)
    {
        return writer.WriteAsync(value);
    }

    public void Dispose()
    {
        writer.Dispose();
        reader.Dispose();
    }
}