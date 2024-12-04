// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Abstraction;

namespace Snap.Hutao.Core.Database.Abstraction;

internal interface ISelectable : IAppDbEntity
{
    bool IsSelected { get; set; }
}