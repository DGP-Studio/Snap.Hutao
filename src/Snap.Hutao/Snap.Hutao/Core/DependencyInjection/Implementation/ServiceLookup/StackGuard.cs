// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection.Implementation.ServiceLookup;

internal sealed class StackGuard
{
    private const int MaxExecutionStackCount = 1024;

    private int executionStackCount;

    public bool TryEnterOnCurrentStack()
    {
        if (RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            return true;
        }

        if (executionStackCount < MaxExecutionStackCount)
        {
            return false;
        }

        throw new InsufficientExecutionStackException();
    }

    public TR RunOnEmptyStack<T1, T2, TR>(Func<T1, T2, TR> action, T1 arg1, T2 arg2)
    {
        // Prefer ValueTuple when available to reduce dependencies on Tuple
        return RunOnEmptyStackCore(
            static s =>
            {
                (Func<T1, T2, TR>, T1, T2) t = ((Func<T1, T2, TR>, T1, T2))s;
                return t.Item1(t.Item2, t.Item3);
            },
            (action, arg1, arg2));
    }

    private TR RunOnEmptyStackCore<TR>(Func<object, TR> action, object state)
    {
        executionStackCount++;

        try
        {
            // Using default scheduler rather than picking up the current scheduler.
            Task<TR> task = Task.Factory.StartNew((Func<object?, TR>)action, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);

            // Avoid AsyncWaitHandle lazy allocation of ManualResetEvent in the rare case we finish quickly.
            if (!task.IsCompleted)
            {
                // Task.Wait has the potential of inlining the task's execution on the current thread; avoid this.
                ((IAsyncResult)task).AsyncWaitHandle.WaitOne();
            }

            // Using awaiter here to propagate original exception
            return task.GetAwaiter().GetResult();
        }
        finally
        {
            executionStackCount--;
        }
    }
}