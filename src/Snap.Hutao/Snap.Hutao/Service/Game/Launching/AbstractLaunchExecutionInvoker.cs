// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Launching;

internal abstract class AbstractLaunchExecutionInvoker
{
    protected ImmutableArray<ILaunchExecutionHandler> Handlers { get; set; }

    public async ValueTask InvokeAsync(LaunchExecutionInvocationContext context)
    {
        string lockTrace = $"{GetType().Name}.{nameof(InvokeAsync)}";
        context.LaunchOptions.TryGetGameFileSystem(lockTrace, out IGameFileSystem? gameFileSystem);
        ArgumentNullException.ThrowIfNull(gameFileSystem);

        using (GameFileSystemReference fileSystemReference = new(gameFileSystem))
        {
            if (context.ViewModel.TargetScheme is not { } targetScheme)
            {
                throw HutaoException.InvalidOperation(SH.ViewModelLaunchGameSchemeNotSelected);
            }

            ITaskContext taskContext = context.ServiceProvider.GetRequiredService<ITaskContext>();
            IProgress<LaunchStatus?> progress = CreateStatusProgress(context.ServiceProvider);

            BeforeLaunchExecutionContext beforeContext = new()
            {
                ViewModel = context.ViewModel,
                Progress = progress,
                ServiceProvider = context.ServiceProvider,
                TaskContext = taskContext,
                FileSystem = fileSystemReference,
                HoyoPlay = context.ServiceProvider.GetRequiredService<IHoyoPlayService>(),
                Messenger = context.ServiceProvider.GetRequiredService<IMessenger>(),
                LaunchOptions = context.LaunchOptions,
                CurrentScheme = context.ViewModel.CurrentScheme,
                TargetScheme = targetScheme,
                Identity = context.Identity,
            };

            foreach (ILaunchExecutionHandler handler in Handlers)
            {
                await handler.BeforeAsync(beforeContext).ConfigureAwait(false);
            }

            fileSystemReference.Exchange(beforeContext.FileSystem);

            IProcess? process = CreateProcess(beforeContext);
            if (process is null)
            {
                return;
            }

            LaunchExecutionContext executionContext = new()
            {
                Progress = progress,
                ServiceProvider = context.ServiceProvider,
                TaskContext = taskContext,
                Messenger = context.ServiceProvider.GetRequiredService<IMessenger>(),
                LaunchOptions = context.LaunchOptions,
                Process = process,
                IsOversea = targetScheme.IsOversea,
            };

            foreach (ILaunchExecutionHandler handler in Handlers)
            {
                await handler.ExecuteAsync(executionContext).ConfigureAwait(false);
            }

            if (process.IsRunning())
            {
                progress.Report(new(SH.ServiceGameLaunchPhaseWaitingProcessExit));
                try
                {
                    await taskContext.SwitchToBackgroundAsync();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    // Access denied, we are in non-elevated process
                    // Just leave and let invoker spin wait
                    SentrySdk.CaptureException(ex);
                    return;
                }
            }

            progress.Report(new(SH.ServiceGameLaunchPhaseProcessExited));

            AfterLaunchExecutionContext afterContext = new()
            {
                TaskContext = taskContext,
            };

            foreach (ILaunchExecutionHandler handler in Handlers)
            {
                await handler.AfterAsync(afterContext).ConfigureAwait(false);
            }
        }
    }

    protected virtual IProcess? CreateProcess(BeforeLaunchExecutionContext beforeContext)
    {
        return LaunchExecutionGameProcessFactory.CreateDefault(beforeContext);
    }

    private static IProgress<LaunchStatus?> CreateStatusProgress(IServiceProvider serviceProvider)
    {
        IProgressFactory progressFactory = serviceProvider.GetRequiredService<IProgressFactory>();
        LaunchStatusOptions options = serviceProvider.GetRequiredService<LaunchStatusOptions>();
        return progressFactory.CreateForMainThread<LaunchStatus?, LaunchStatusOptions>(static (status, options) => options.LaunchStatus = status, options);
    }
}