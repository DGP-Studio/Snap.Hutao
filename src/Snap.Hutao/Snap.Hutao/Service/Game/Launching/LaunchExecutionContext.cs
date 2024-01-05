// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.ViewModel.Game;

namespace Snap.Hutao.Service.Game.Launching;

[ConstructorGenerated]
internal sealed partial class LaunchExecutionContext
{
    private readonly ILogger<LaunchExecutionContext> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly LaunchOptions options;

    public LaunchExecutionResult Result { get; } = new();

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public ITaskContext TaskContext { get => taskContext; }

    public ILogger Logger { get => logger; }

    public LaunchOptions Options { get => options; }

    public IViewModelSupportLaunchExecution ViewModel { get; set; } = default!;

    public LaunchScheme Scheme { get; set; } = default!;

    public GameAccount? Account { get; set; }

    public IProgress<LaunchStatus> Progress { get; set; }
}