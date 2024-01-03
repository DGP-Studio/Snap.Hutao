// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;

namespace Snap.Hutao.ViewModel.Game;

internal sealed class GameAccountFilter
{
    private readonly SchemeType? type;

    public GameAccountFilter(SchemeType? type)
    {
        this.type = type;
    }

    public bool Filter(object? item)
    {
        if (type is null)
        {
            return true;
        }

        return item is GameAccount account && account.Type == type;
    }
}