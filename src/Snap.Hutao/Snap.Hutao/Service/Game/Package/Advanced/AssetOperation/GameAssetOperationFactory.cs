// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Service.Game.Package.Advanced.AssetOperation;

[Service(ServiceLifetime.Transient, typeof(IDriverMediaTypeAwareFactory<IGameAssetOperation>))]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class GameAssetOperationFactory : DriverMediaTypeAwareFactory<IGameAssetOperation, GameAssetOperationSSD, GameAssetOperationHDD>;