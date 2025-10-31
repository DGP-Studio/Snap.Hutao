// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

[Service(ServiceLifetime.Transient, typeof(IOverseaSupportFactory<IHomeClient>))]
internal sealed partial class HomeClientFactory : OverseaSupportFactory<IHomeClient, HomeClient, HomeClientOversea>
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial HomeClientFactory(IServiceProvider serviceProvider);
}