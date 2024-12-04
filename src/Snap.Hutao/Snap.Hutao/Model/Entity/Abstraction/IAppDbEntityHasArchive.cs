// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Entity.Abstraction;

internal interface IAppDbEntityHasArchive : IAppDbEntity
{
    Guid ArchiveId { get; }
}