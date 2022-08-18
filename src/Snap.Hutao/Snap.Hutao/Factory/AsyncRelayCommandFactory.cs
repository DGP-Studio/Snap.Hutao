// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.Abstraction;

namespace Snap.Hutao.Factory;

/// <inheritdoc cref="IAsyncRelayCommandFactory"/>
[Injection(InjectAs.Transient, typeof(IAsyncRelayCommandFactory))]
internal class AsyncRelayCommandFactory : IAsyncRelayCommandFactory
{
    private readonly ILogger logger;

    /// <summary>
    /// 构造一个新的异步命令工厂
    /// </summary>
    /// <param name="logger">日志器</param>
    public AsyncRelayCommandFactory(ILogger<AsyncRelayCommandFactory> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public AsyncRelayCommand<T> Create<T>(Func<T?, Task> execute)
    {
        return Register(new AsyncRelayCommand<T>(execute));
    }

    /// <inheritdoc/>
    public AsyncRelayCommand<T> Create<T>(Func<T?, CancellationToken, Task> cancelableExecute)
    {
        return Register(new AsyncRelayCommand<T>(cancelableExecute));
    }

    /// <inheritdoc/>
    public AsyncRelayCommand<T> Create<T>(Func<T?, Task> execute, Predicate<T?> canExecute)
    {
        return Register(new AsyncRelayCommand<T>(execute, canExecute));
    }

    /// <inheritdoc/>
    public AsyncRelayCommand<T> Create<T>(Func<T?, CancellationToken, Task> cancelableExecute, Predicate<T?> canExecute)
    {
        return Register(new AsyncRelayCommand<T>(cancelableExecute, canExecute));
    }

    /// <inheritdoc/>
    public AsyncRelayCommand Create(Func<Task> execute)
    {
        return Register(new AsyncRelayCommand(execute));
    }

    /// <inheritdoc/>
    public AsyncRelayCommand Create(Func<CancellationToken, Task> cancelableExecute)
    {
        return Register(new AsyncRelayCommand(cancelableExecute));
    }

    /// <inheritdoc/>
    public AsyncRelayCommand Create(Func<Task> execute, Func<bool> canExecute)
    {
        return Register(new AsyncRelayCommand(execute, canExecute));
    }

    /// <inheritdoc/>
    public AsyncRelayCommand Create(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute)
    {
        return Register(new AsyncRelayCommand(cancelableExecute, canExecute));
    }

    private AsyncRelayCommand Register(AsyncRelayCommand command)
    {
        ReportException(command);
        return command;
    }

    private AsyncRelayCommand<T> Register<T>(AsyncRelayCommand<T> command)
    {
        ReportException(command);
        return command;
    }

    private void ReportException(IAsyncRelayCommand command)
    {
        command.PropertyChanged += (sender, args) =>
        {
            if (sender is IAsyncRelayCommand asyncRelayCommand)
            {
                if (args.PropertyName == nameof(AsyncRelayCommand.ExecutionTask))
                {
                    if (asyncRelayCommand.ExecutionTask?.Exception is AggregateException exception)
                    {
                        Exception baseException = exception.GetBaseException();
                        logger.LogError(EventIds.AsyncCommandException, baseException, "{name} Exception", nameof(asyncRelayCommand));
                    }
                }
            }
        };
    }
}