// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.Frozen;

namespace Snap.Hutao.Model.Metadata.Item;

internal sealed class RotationalMaterialIdEntry
{
    public RotationalMaterialIdEntry(DaysOfWeek daysOfWeek, MaterialId green, MaterialId blue, MaterialId purple, MaterialId orange)
        : this(daysOfWeek, green, blue, purple)
    {
        DaysOfWeek = daysOfWeek;
        Orange = orange;
        Purple = purple;
        Blue = blue;
        Green = green;

        Set = [green, blue, purple, orange];
    }

    public RotationalMaterialIdEntry(DaysOfWeek daysOfWeek, MaterialId green, MaterialId blue, MaterialId purple)
    {
        DaysOfWeek = daysOfWeek;
        Purple = purple;
        Blue = blue;
        Green = green;

        Set = [green, blue, purple];
    }

    public DaysOfWeek DaysOfWeek { get; }

    public MaterialId Orange { get; }

    public MaterialId Purple { get; }

    public MaterialId Blue { get; }

    public MaterialId Green { get; }

    public MaterialId Highest { get => Orange != 0U ? Orange : Purple; }

    public FrozenSet<MaterialId> Set { get; }

    public IEnumerable<MaterialId> Enumerate()
    {
        yield return Green;
        yield return Blue;
        yield return Purple;

        if (Orange != 0U)
        {
            yield return Orange;
        }
    }
}