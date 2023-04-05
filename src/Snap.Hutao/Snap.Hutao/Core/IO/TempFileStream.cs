// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Core.IO;

/// <summary>
/// 临时文件流
/// </summary>
internal sealed class TempFileStream : Stream
{
    private readonly string path;
    private readonly FileStream stream;

    /// <summary>
    /// 构造一个新的临时的文件流
    /// </summary>
    /// <param name="mode">文件模式</param>
    /// <param name="access">访问方式</param>
    public TempFileStream(FileMode mode, FileAccess access)
    {
        path = Path.GetTempFileName();
        stream = File.Open(path, mode, access);
    }

    /// <inheritdoc/>
    public override bool CanRead { get => stream.CanRead; }

    /// <inheritdoc/>
    public override bool CanSeek { get => stream.CanSeek; }

    /// <inheritdoc/>
    public override bool CanWrite { get => stream.CanWrite; }

    /// <inheritdoc/>
    public override long Length { get => stream.Length; }

    /// <inheritdoc/>
    public override long Position { get => stream.Position; set => stream.Position = value; }

    /// <inheritdoc/>
    public override void Flush()
    {
        stream.Flush();
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        return stream.Read(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        return stream.Seek(offset, origin);
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        stream.SetLength(value);
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        stream.Write(buffer, offset, count);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        stream.Dispose();
        File.Delete(path);
    }
}