// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

[Injection(InjectAs.Transient, typeof(IOverseaSupportFactory<IUserClient>))]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class UserClientFactory : OverseaSupportFactory<IUserClient, UserClient, UserClientOversea>
{
}