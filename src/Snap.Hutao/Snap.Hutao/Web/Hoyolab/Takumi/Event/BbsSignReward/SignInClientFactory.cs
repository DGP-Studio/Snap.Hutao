// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

[Service(ServiceLifetime.Transient, typeof(IOverseaSupportFactory<ISignInClient>))]
internal sealed partial class SignInClientFactory : OverseaSupportFactory<ISignInClient, SignInClient, SignInClientOversea>
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial SignInClientFactory(IServiceProvider serviceProvider);
}