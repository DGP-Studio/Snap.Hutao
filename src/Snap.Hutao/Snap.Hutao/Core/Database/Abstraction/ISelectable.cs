// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Database.Abstraction;

internal interface ISelectable
{
    Guid InnerId { get; }

    bool IsSelected { get; set; }
}