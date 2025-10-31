// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Miyolive;

[Service(ServiceLifetime.Transient, typeof(IOverseaSupportFactory<IMiyoliveClient>))]
internal sealed partial class MiyoliveClientFactory : OverseaSupportFactory<IMiyoliveClient, MiyoliveClient, MiyoliveClientOversea>
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial MiyoliveClientFactory(IServiceProvider serviceProvider);
}