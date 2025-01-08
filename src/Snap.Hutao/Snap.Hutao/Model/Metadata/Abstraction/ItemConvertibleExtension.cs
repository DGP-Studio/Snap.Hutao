// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Model.Metadata.Abstraction;

internal static class ItemConvertibleExtension
{
    private static readonly ConditionalWeakTable<IItemConvertible, Model.Item> Items = [];

    public static Model.Item GetOrCreateItem(this IItemConvertible source)
    {
        return Items.GetValue(source, value => value.ToItem<Model.Item>());
    }
}