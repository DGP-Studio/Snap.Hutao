// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using Snap.Hutao.Control.Extension;

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
        RunCoreAsync().SafeForget();
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

    private async ValueTask RunCoreAsync()
    {
        using (PeriodicTimer timer = new(Period))
        {
            do
            {
                if (!IsAttached)
                {
                    break;
                }

                // TODO: Reconsider approach to get the ServiceProvider
                ITaskContext taskContext = Ioc.Default.GetRequiredService<ITaskContext>();
                await taskContext.SwitchToMainThreadAsync();
                TryExecuteCommand();

                await taskContext.SwitchToBackgroundAsync();
                try
                {
                    using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(periodicTimerStopCts.Token, periodicTimerStopCts.Token))
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

                acutalThemeChangedCts.Dispose();
                acutalThemeChangedCts = new();
                periodicTimerStopCts.Dispose();
                periodicTimerStopCts = new();
            }
            while (true);
        }
    }
}