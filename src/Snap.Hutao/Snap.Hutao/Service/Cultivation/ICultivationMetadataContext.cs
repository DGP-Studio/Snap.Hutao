// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Metadata.ContextAbstraction;

namespace Snap.Hutao.Service.Cultivation;

internal interface ICultivationMetadataContext : IMetadataContext,
    IMetadataArrayMaterialSource,
    IMetadataDictionaryIdMaterialSource,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource;