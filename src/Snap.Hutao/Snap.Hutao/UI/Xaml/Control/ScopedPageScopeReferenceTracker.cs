// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control;

/// <summary>
/// By injecting into services, we take dvantage of the fact that
/// IServiceProvider disposes all injected services when it is disposed.
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IScopedPageScopeReferenceTracker))]
internal sealed partial class ScopedPageScopeReferenceTracker : IScopedPageScopeReferenceTracker, IDisposable
{
    private readonly IServiceProvider serviceProvider;

    private readonly WeakReference<IServiceScope> previousScopeReference = new(default!);

    public void Dispose()
    {
        DisposePreviousScope();
    }

    public IServiceScope CreateScope()
    {
        IServiceScope currentScope = serviceProvider.CreateScope();

        // In case previous one is not disposed.
        DisposePreviousScope();
        previousScopeReference.SetTarget(currentScope);
        return currentScope;
    }

    private void DisposePreviousScope()
    {
        if (previousScopeReference.TryGetTarget(out IServiceScope? scope))
        {
            scope.Dispose();
        }
    }
}