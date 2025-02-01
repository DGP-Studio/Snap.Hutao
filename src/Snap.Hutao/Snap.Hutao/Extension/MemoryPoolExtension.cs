// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using System.Buffers;

namespace Snap.Hutao.Extension;

internal static class MemoryPoolExtension
{
    public static IMemoryOwner<T> RentExactly<T>(this MemoryPool<T> memoryPool, int bufferSize)
    {
        IMemoryOwner<T> memoryOwner = memoryPool.Rent(bufferSize);
        return new ExactSizedMemoryOwner<T>(memoryOwner, bufferSize);
    }

    private sealed class ExactSizedMemoryOwner<T> : IMemoryOwner<T>
    {
        private IMemoryOwner<T> owner;
        private int bufferSize;

        public ExactSizedMemoryOwner(IMemoryOwner<T> owner, int bufferSize)
        {
            this.owner = owner;
            this.bufferSize = bufferSize;
        }

        public Memory<T> Memory => owner.Memory.Slice(0, bufferSize);

        public void Dispose()
        {
            owner.Dispose();
        }
    }
}