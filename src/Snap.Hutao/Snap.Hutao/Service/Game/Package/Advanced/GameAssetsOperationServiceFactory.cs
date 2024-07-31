// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[Injection(InjectAs.Transient, typeof(IDriverMediaTypeAwareFactory<IGameAssetsOperationService>))]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class GameAssetsOperationServiceFactory : DriverMediaTypeAwareFactory<IGameAssetsOperationService, GameAssetsOperationServiceSSD, GameAssetsOperationServiceHDD>
{
}