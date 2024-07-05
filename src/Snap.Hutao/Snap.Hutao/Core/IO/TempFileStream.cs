// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

internal sealed class TempFileStream : Stream
{
    private readonly string path;
    private readonly FileStream stream;

    public TempFileStream(FileMode mode, FileAccess access)
    {
        path = Path.GetTempFileName();
        stream = File.Open(path, mode, access);
    }

    public override bool CanRead { get => stream.CanRead; }

    public override bool CanSeek { get => stream.CanSeek; }

    public override bool CanWrite { get => stream.CanWrite; }

    public override long Length { get => stream.Length; }

    public override long Position { get => stream.Position; set => stream.Position = value; }

    public override void Flush()
    {
        stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        stream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            stream.Dispose();
            File.Delete(path);
        }

        base.Dispose(disposing);
    }
}