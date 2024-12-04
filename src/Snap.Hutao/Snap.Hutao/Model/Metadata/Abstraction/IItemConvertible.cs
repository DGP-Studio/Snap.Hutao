// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Abstraction;

internal interface IItemConvertible
{
    TItem ToItem<TItem>()
        where TItem : Model.Item, new();
}