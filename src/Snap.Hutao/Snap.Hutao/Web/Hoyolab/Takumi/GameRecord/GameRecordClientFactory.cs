// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient, typeof(IOverseaSupportFactory<IGameRecordClient>))]
internal sealed partial class GameRecordClientFactory : OverseaSupportFactory<IGameRecordClient, GameRecordClient, GameRecordClientOversea>
{
}