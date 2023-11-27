// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Process;

internal sealed class GameProcessTracker : IDisposable
{
    private readonly Stack<IDisposable> disposables = [];

    public TDisposable Track<TDisposable>(TDisposable disposable)
        where TDisposable : IDisposable
    {
        disposables.Push(disposable);
        return disposable;
    }

    public void Dispose()
    {
        while (disposables.TryPop(out IDisposable? disposable))
        {
            disposable.Dispose();
        }
    }
}