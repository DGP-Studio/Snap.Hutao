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
    private CancellationTokenSource actualThemeChangedCts = new();
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
        actualThemeChangedCts.Dispose();

        return true;
    }

    protected override void OnAssociatedObjectLoaded()
    {
        PrivateRunAsync().SafeForget();
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        actualThemeChangedCts.Cancel();
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
    private async Task PrivateRunAsync()
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
                    using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(actualThemeChangedCts.Token, periodicTimerStopCts.Token))
                    {
                        await timer.WaitForNextTickAsync(linkedCts.Token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    if (periodicTimerStopCts.IsCancellationRequested)
                    {
                        break;
                    }
                }

                taskContext.BeginInvokeOnMainThread(TryExecuteCommand);

                actualThemeChangedCts.Dispose();
                actualThemeChangedCts = new();
                periodicTimerStopCts.Dispose();
                periodicTimerStopCts = new();
            }
            while (true);
        }
    }
}