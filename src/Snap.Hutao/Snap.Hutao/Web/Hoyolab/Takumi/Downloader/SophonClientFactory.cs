// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Downloader;

[Injection(InjectAs.Transient, typeof(IOverseaSupportFactory<ISophonClient>))]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class SophonClientFactory : OverseaSupportFactory<ISophonClient, SophonClient, SophonClientOversea>
{
}
