using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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