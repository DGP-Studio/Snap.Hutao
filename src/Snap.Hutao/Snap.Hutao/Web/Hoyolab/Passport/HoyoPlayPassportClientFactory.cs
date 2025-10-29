// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[GeneratedConstructor(CallBaseConstructor = true)]
[Service(ServiceLifetime.Transient, typeof(IOverseaSupportFactory<IHoyoPlayPassportClient>))]
internal sealed partial class HoyoPlayPassportClientFactory : OverseaSupportFactory<IHoyoPlayPassportClient, HoyoPlayPassportClient, HoyoPlayPassportClientOversea>;