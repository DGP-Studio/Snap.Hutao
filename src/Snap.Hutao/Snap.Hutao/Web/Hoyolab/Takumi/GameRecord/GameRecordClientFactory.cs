// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

[Service(ServiceLifetime.Transient, typeof(IOverseaSupportFactory<IGameRecordClient>))]
internal sealed partial class GameRecordClientFactory : OverseaSupportFactory<IGameRecordClient, GameRecordClient, GameRecordClientOversea>
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial GameRecordClientFactory(IServiceProvider serviceProvider);
}