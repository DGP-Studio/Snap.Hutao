// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using System.Buffers;

namespace Snap.Hutao.Service.Game.Automation.ScreenCapture;

internal sealed class GameScreenCaptureMemoryPool : MemoryPool<byte>
{
    private static readonly LazySlim<GameScreenCaptureMemoryPool> LazyShared = new(() => new());

    private readonly object syncRoot = new();
    private readonly LinkedList<GameScreenCaptureBuffer> unrentedBuffers = [];
    private readonly LinkedList<GameScreenCaptureBuffer> rentedBuffers = [];

    private int bufferCount;

    public static new GameScreenCaptureMemoryPool Shared { get => LazyShared.Value; }

    public override int MaxBufferSize { get => Array.MaxLength; }

    public int BufferCount { get => bufferCount; }

    public override IMemoryOwner<byte> Rent(int minBufferSize = -1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minBufferSize);

        lock (syncRoot)
        {
            foreach (GameScreenCaptureBuffer buffer in unrentedBuffers)
            {
                if (buffer.Memory.Length >= minBufferSize)
                {
                    unrentedBuffers.Remove(buffer);
                    rentedBuffers.AddLast(buffer);
                    return buffer;
                }
            }

            GameScreenCaptureBuffer newBuffer = new(this, minBufferSize);
            rentedBuffers.AddLast(newBuffer);
            ++bufferCount;
            return newBuffer;
        }
    }

    protected override void Dispose(bool disposing)
    {
        lock (syncRoot)
        {
            if (rentedBuffers.Count > 0)
            {
                HutaoException.InvalidOperation("There are still rented buffers.");
            }
        }
    }

    internal sealed class GameScreenCaptureBuffer : IMemoryOwner<byte>
    {
        private readonly GameScreenCaptureMemoryPool pool;
        private readonly byte[] buffer;

        public GameScreenCaptureBuffer(GameScreenCaptureMemoryPool pool, int bufferSize)
        {
            this.pool = pool;
            buffer = GC.AllocateUninitializedArray<byte>(bufferSize);
        }

        public Memory<byte> Memory { get => buffer.AsMemory(); }

        public void Dispose()
        {
            lock (pool.syncRoot)
            {
                pool.rentedBuffers.Remove(this);
                pool.unrentedBuffers.AddLast(this);
            }
        }
    }
}