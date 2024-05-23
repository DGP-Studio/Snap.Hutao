// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.Factory.Progress;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IProgressFactory))]
internal sealed partial class ProgressFactory : IProgressFactory
{
    private readonly ITaskContext taskContext;

    public IProgress<T> CreateForMainThread<T>(Action<T> handler)
    {
        if (taskContext is not ITaskContextUnsafe @unsafe)
        {
            throw HutaoException.NotSupported();
        }

        return new DispatcherQueueProgress<T>(handler, @unsafe.DispatcherQueue);
    }
}