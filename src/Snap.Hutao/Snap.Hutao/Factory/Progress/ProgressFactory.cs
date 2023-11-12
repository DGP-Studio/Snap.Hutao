// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Factory.Progress;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IProgressFactory))]
internal sealed partial class ProgressFactory : IProgressFactory
{
    private readonly ITaskContext taskContext;

    public IProgress<T> CreateForMainThread<T>(Action<T> handler)
    {
        return new DispatcherQueueProgress<T>(handler, taskContext.GetSynchronizationContext());
    }
}