// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Factory.Abstraction;

namespace Snap.Hutao.Factory;

/// <inheritdoc cref="IAsyncRelayCommandFactory"/>
[Injection(InjectAs.Transient, typeof(IAsyncRelayCommandFactory))]
internal class AsyncRelayCommandFactory : IAsyncRelayCommandFactory
{
    /// <inheritdoc/>
    public AsyncRelayCommand<T> Create<T>(Func<T?, Task> execute)
    {
        return new AsyncRelayCommand<T>(execute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    /// <inheritdoc/>
    public AsyncRelayCommand<T> Create<T>(Func<T?, CancellationToken, Task> cancelableExecute)
    {
        return new AsyncRelayCommand<T>(cancelableExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    /// <inheritdoc/>
    public AsyncRelayCommand<T> Create<T>(Func<T?, Task> execute, Predicate<T?> canExecute)
    {
        return new AsyncRelayCommand<T>(execute, canExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    /// <inheritdoc/>
    public AsyncRelayCommand<T> Create<T>(Func<T?, CancellationToken, Task> cancelableExecute, Predicate<T?> canExecute)
    {
        return new AsyncRelayCommand<T>(cancelableExecute, canExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    /// <inheritdoc/>
    public AsyncRelayCommand Create(Func<Task> execute)
    {
        return new AsyncRelayCommand(execute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    /// <inheritdoc/>
    public AsyncRelayCommand Create(Func<CancellationToken, Task> cancelableExecute)
    {
        return new AsyncRelayCommand(cancelableExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    /// <inheritdoc/>
    public AsyncRelayCommand Create(Func<Task> execute, Func<bool> canExecute)
    {
        return new AsyncRelayCommand(execute, canExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    /// <inheritdoc/>
    public AsyncRelayCommand Create(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute)
    {
        return new AsyncRelayCommand(cancelableExecute, canExecute, AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }
}
