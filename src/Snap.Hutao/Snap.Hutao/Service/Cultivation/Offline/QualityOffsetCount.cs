// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Service.Cultivation.Offline;

internal readonly struct QualityOffsetCount : IDeconstruct<uint, uint>
{
    public readonly uint QualityOffset;
    public readonly uint Count;

    public QualityOffsetCount(uint qualityOffset, uint count)
    {
        QualityOffset = qualityOffset;
        Count = count;
    }

    public static implicit operator QualityOffsetCount((uint Offset, uint Count) tuple)
    {
        return new(tuple.Offset, tuple.Count);
    }

    public void Deconstruct(out uint qualityOffset, out uint count)
    {
        qualityOffset = QualityOffset;
        count = Count;
    }

    public (uint FinalId, uint Count) Positive(uint baseId)
    {
        return (baseId + QualityOffset, Count);
    }

    public (uint FinalId, uint Count) Negative(uint baseId)
    {
        return (baseId - QualityOffset, Count);
    }
}