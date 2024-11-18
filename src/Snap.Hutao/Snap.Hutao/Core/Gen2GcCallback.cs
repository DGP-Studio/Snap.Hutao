// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core;

internal sealed class Gen2GcCallback : CriticalFinalizerObject
{
    private readonly Func<bool>? callback0;
    private readonly Func<object, bool>? callback1;
    private GCHandle weakTargetObj;

    private Gen2GcCallback(Func<bool> callback)
    {
        callback0 = callback;
    }

    private Gen2GcCallback(Func<object, bool> callback, object targetObj)
    {
        callback1 = callback;
        weakTargetObj = GCHandle.Alloc(targetObj, GCHandleType.Weak);
    }

    ~Gen2GcCallback()
    {
        if (weakTargetObj.IsAllocated)
        {
            // Check to see if the target object is still alive.
            object? targetObj = weakTargetObj.Target;
            if (targetObj is null)
            {
                // The target object is dead, so this callback object is no longer needed.
                weakTargetObj.Free();
                return;
            }

            // Execute the callback method.
            try
            {
                Debug.Assert(callback1 is not null);
                if (!callback1(targetObj))
                {
                    // If the callback returns false, this callback object is no longer needed.
                    weakTargetObj.Free();
                    return;
                }
            }

            // ReSharper disable once RedundantCatchClause
            catch
            {
                // Ensure that we still get a chance to resurrect this object, even if the callback throws an exception.
#if DEBUG
                // Except in DEBUG, as we really shouldn't be hitting any exceptions here.
                throw;
#endif
            }
        }
        else
        {
            // Execute the callback method.
            try
            {
                Debug.Assert(callback0 is not null);
                if (!callback0())
                {
                    // If the callback returns false, this callback object is no longer needed.
                    return;
                }
            }

            // ReSharper disable once RedundantCatchClause
            catch
            {
                // Ensure that we still get a chance to resurrect this object, even if the callback throws an exception.
#if DEBUG
                // Except in DEBUG, as we really shouldn't be hitting any exceptions here.
                throw;
#endif
            }
        }

        // Resurrect ourselves by re-registering for finalization.
        GC.ReRegisterForFinalize(this);
    }

    /// <summary>
    /// Schedule 'callback' to be called in the next GC.  If the callback returns true it is
    /// rescheduled for the next Gen 2 GC.  Otherwise the callbacks stop.
    /// </summary>
    /// <param name="callback">callback</param>
    public static void Register(Func<bool> callback)
    {
        // Create a unreachable object that remembers the callback function and target object.
        _ = new Gen2GcCallback(callback);
    }

    /// <summary>
    /// Schedule 'callback' to be called in the next GC.  If the callback returns true it is
    /// rescheduled for the next Gen 2 GC.  Otherwise the callbacks stop.
    ///
    /// NOTE: This callback will be kept alive until either the callback function returns false,
    /// or the target object dies.
    /// </summary>
    /// <param name="callback">callback</param>
    /// <param name="targetObj">target</param>
    public static void Register(Func<object, bool> callback, object targetObj)
    {
        // Create a unreachable object that remembers the callback function and target object.
        _ = new Gen2GcCallback(callback, targetObj);
    }
}