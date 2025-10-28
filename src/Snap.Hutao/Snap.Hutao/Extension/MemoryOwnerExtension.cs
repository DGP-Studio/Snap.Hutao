// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;

namespace Snap.Hutao.Extension;

internal static partial class MemoryOwnerExtension
{
    extension<T>(IMemoryOwner<T>)
    {
        public static IMemoryOwner<T> Empty
        {
            get => EmptyMemoryOwner<T>.Instance;
        }
    }

    private sealed partial class EmptyMemoryOwner<T> : IMemoryOwner<T>
    {
        public static readonly EmptyMemoryOwner<T> Instance = new();

        public Memory<T> Memory { get => Memory<T>.Empty; }

        public void Dispose()
        {
            // Do nothing, so that can be disposed multiple times safely.
        }
    }
}