// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Abstraction;

internal interface IAppInfrastructureService : IAppService
{
    IServiceProvider ServiceProvider { get; }
}