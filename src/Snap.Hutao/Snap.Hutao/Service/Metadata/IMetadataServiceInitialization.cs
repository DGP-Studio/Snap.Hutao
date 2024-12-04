// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Metadata;

internal interface IMetadataServiceInitialization
{
    ValueTask InitializeInternalAsync(CancellationToken token = default);
}