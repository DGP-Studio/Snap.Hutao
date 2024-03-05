// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using Snap.Hutao.Control.Extension;

namespace Snap.Hutao.Control.Behavior;

[DependencyProperty("Period", typeof(TimeSpan))]
[DependencyProperty("Command", typeof(ICommand))]
[DependencyProperty("CommandParameter", typeof(object))]
internal sealed partial class PeriodicInvokeCommandOrOnActualThemeChangedBehavior : BehaviorBase<FrameworkElement>, IDisposable
{
    private TaskCompletionSource acutalThemeChangedTaskCompletionSource = new();
    private CancellationTokenSource periodicTimerCancellationTokenSource = new();

    public void Dispose()
    {
        periodicTimerCancellationTokenSource.Dispose();
    }

    protected override bool Initialize()
    {
        AssociatedObject.ActualThemeChanged += OnActualThemeChanged;
        return true;
    }

    protected override void OnAssociatedObjectLoaded()
    {
        RunCoreAsync().SafeForget();
    }

    protected override bool Uninitialize()
    {
        AssociatedObject.ActualThemeChanged -= OnActualThemeChanged;
        return true;
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        acutalThemeChangedTaskCompletionSource.TrySetResult();
        periodicTimerCancellationTokenSource.Cancel();
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

                ITaskContext taskContext = Ioc.Default.GetRequiredService<ITaskContext>();
                await taskContext.SwitchToMainThreadAsync();
                TryExecuteCommand();

                await taskContext.SwitchToBackgroundAsync();
                try
                {
                    Task nextTickTask = timer.WaitForNextTickAsync(periodicTimerCancellationTokenSource.Token).AsTask();
                    await Task.WhenAny(nextTickTask, acutalThemeChangedTaskCompletionSource.Task).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }

                acutalThemeChangedTaskCompletionSource = new();
                periodicTimerCancellationTokenSource = new();
            }
            while (true);
        }
    }
}