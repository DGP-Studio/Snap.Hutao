// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Input;

namespace Snap.Hutao.UI.Xaml.Behavior;

[SuppressMessage("", "CA1001")]
[DependencyProperty("Period", typeof(TimeSpan))]
[DependencyProperty("Command", typeof(ICommand))]
[DependencyProperty("CommandParameter", typeof(object))]
internal sealed partial class PeriodicInvokeCommandOrOnActualThemeChangedBehavior : BehaviorBase<FrameworkElement>
{
    private CancellationTokenSource acutalThemeChangedCts = new();
    private CancellationTokenSource periodicTimerStopCts = new();

    protected override bool Initialize()
    {
        AssociatedObject.ActualThemeChanged += OnActualThemeChanged;
        return true;
    }

    protected override bool Uninitialize()
    {
        periodicTimerStopCts.Cancel();
        periodicTimerStopCts.Dispose();

        AssociatedObject.ActualThemeChanged -= OnActualThemeChanged;
        acutalThemeChangedCts.Dispose();

        return true;
    }

    protected override void OnAssociatedObjectLoaded()
    {
        _ = RunCoreAsync();
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        acutalThemeChangedCts.Cancel();
    }

    private void TryExecuteCommand()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        Command.TryExecute(CommandParameter);
    }

    [SuppressMessage("", "SH003")]
    private async Task RunCoreAsync()
    {
        using (PeriodicTimer timer = new(Period))
        {
            do
            {
                if (!IsAttached)
                {
                    break;
                }

                ITaskContext taskContext = TaskContext.GetForDispatcherQueue(AssociatedObject.DispatcherQueue);
                await taskContext.SwitchToBackgroundAsync();
                try
                {
                    using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(acutalThemeChangedCts.Token, periodicTimerStopCts.Token))
                    {
                        await timer.WaitForNextTickAsync(linkedCts.Token).ConfigureAwait(false);
                        taskContext.BeginInvokeOnMainThread(TryExecuteCommand);
                    }
                }
                catch (OperationCanceledException)
                {
                    if (periodicTimerStopCts.IsCancellationRequested)
                    {
                        break;
                    }
                }

                acutalThemeChangedCts.Dispose();
                acutalThemeChangedCts = new();
                periodicTimerStopCts.Dispose();
                periodicTimerStopCts = new();
            }
            while (true);
        }
    }
}