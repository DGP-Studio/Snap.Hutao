// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[Service(ServiceLifetime.Transient, typeof(IOverseaSupportFactory<IPassportClient>))]
internal sealed partial class PassportClientFactory : OverseaSupportFactory<IPassportClient, PassportClient, PassportClientOversea>
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial PassportClientFactory(IServiceProvider serviceProvider);
}