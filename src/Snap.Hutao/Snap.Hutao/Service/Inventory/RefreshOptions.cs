// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Yae;
using Snap.Hutao.ViewModel.Game;

namespace Snap.Hutao.Service.Inventory;

internal sealed class RefreshOptions
{
    private RefreshOptions()
    {
    }

    public required RefreshOptionKind Kind { get; init; }

    public required CultivateProject Project { get; init; }

    public required ICultivationMetadataContext? MetadataContext { get; init; }

    public required IYaeService? YaeService { get; init; }

    public required IViewModelSupportLaunchExecution? ViewModelSupportLaunchExecution { get; init; }

    public static RefreshOptions CreateForWebCalculator(CultivateProject project, ICultivationMetadataContext context)
    {
        return new()
        {
            Kind = RefreshOptionKind.WebCalculator,
            Project = project,
            MetadataContext = context,
            YaeService = default,
            ViewModelSupportLaunchExecution = default,
        };
    }

    public static RefreshOptions CreateForEmbeddedYae(CultivateProject project, IYaeService yaeService, IViewModelSupportLaunchExecution viewModel)
    {
        return new()
        {
            Kind = RefreshOptionKind.EmbeddedYae,
            Project = project,
            MetadataContext = default,
            YaeService = yaeService,
            ViewModelSupportLaunchExecution = viewModel,
        };
    }
}