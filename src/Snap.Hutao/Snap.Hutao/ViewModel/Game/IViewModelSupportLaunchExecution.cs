// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Package;
using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.ViewModel.Game;

internal interface IViewModelSupportLaunchExecution
{
    LaunchScheme? TargetScheme { get; }

    LaunchScheme? CurrentScheme { get; }

    GameAccount? GameAccount { get; }

    ValueTask<BlockDeferral<PackageConvertStatus>> CreateConvertBlockDeferralAsync();
}