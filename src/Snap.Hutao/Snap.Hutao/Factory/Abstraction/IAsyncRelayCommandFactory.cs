// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;

namespace Snap.Hutao.Factory.Abstraction;

/// <summary>
/// Factory for creating <see cref="AsyncRelayCommand"/> with additional processing.
/// </summary>
public interface IAsyncRelayCommandFactory
{
    /// <summary>
    /// Create a reference to AsyncRelayCommand.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <returns>AsyncRelayCommand.</returns>
    AsyncRelayCommand Create(Func<CancellationToken, Task> cancelableExecute);

    /// <summary>
    /// Create a reference to AsyncRelayCommand.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <returns>AsyncRelayCommand.</returns>
    AsyncRelayCommand Create(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute);

    /// <summary>
    /// Create a reference to AsyncRelayCommand.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <returns>AsyncRelayCommand.</returns>
    AsyncRelayCommand Create(Func<Task> execute);

    /// <summary>
    /// Create a reference to AsyncRelayCommand.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <returns>AsyncRelayCommand.</returns>
    AsyncRelayCommand Create(Func<Task> execute, Func<bool> canExecute);

    /// <summary>
    /// Create a reference to AsyncRelayCommand.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <returns>AsyncRelayCommand.</returns>
    AsyncRelayCommand<T> Create<T>(Func<T?, CancellationToken, Task> cancelableExecute);

    /// <summary>
    /// Create a reference to AsyncRelayCommand.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <returns>AsyncRelayCommand.</returns>
    AsyncRelayCommand<T> Create<T>(Func<T?, CancellationToken, Task> cancelableExecute, Predicate<T?> canExecute);

    /// <summary>
    /// Create a reference to AsyncRelayCommand.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    /// <param name="execute">The execution logic.</param>
    /// <returns>AsyncRelayCommand.</returns>
    AsyncRelayCommand<T> Create<T>(Func<T?, Task> execute);

    /// <summary>
    /// Create a reference to AsyncRelayCommand.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <returns>AsyncRelayCommand.</returns>
    AsyncRelayCommand<T> Create<T>(Func<T?, Task> execute, Predicate<T?> canExecute);
}
