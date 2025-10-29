// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Downloader;

[Service(ServiceLifetime.Transient, typeof(IOverseaSupportFactory<ISophonClient>))]
[GeneratedConstructor(CallBaseConstructor = true)]
internal sealed partial class SophonClientFactory : OverseaSupportFactory<ISophonClient, SophonClient, SophonClientOversea>;