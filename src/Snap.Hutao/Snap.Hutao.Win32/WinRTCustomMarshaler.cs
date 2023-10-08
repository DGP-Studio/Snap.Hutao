using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Windows.Win32.CsWin32.InteropServices;

internal class WinRTCustomMarshaler : ICustomMarshaler
{
    private static readonly string? AssemblyFullName = typeof(Windows.Foundation.IMemoryBuffer).Assembly.FullName;

    private readonly string className;
    private bool lookedForFromAbi;
    private MethodInfo? fromAbiMethod;

    private WinRTCustomMarshaler(string className)
    {
        this.className = className;
    }

    public static ICustomMarshaler GetInstance(string cookie)
    {
        return new WinRTCustomMarshaler(cookie);
    }

    public void CleanUpManagedData(object ManagedObj)
    {
    }

    public void CleanUpNativeData(nint pNativeData)
    {
        Marshal.Release(pNativeData);
    }

    public int GetNativeDataSize()
    {
        throw new NotSupportedException();
    }

    public nint MarshalManagedToNative(object ManagedObj)
    {
        throw new NotSupportedException();
    }

    public object MarshalNativeToManaged(nint thisPtr)
    {
        return className switch
        {
            "Windows.System.DispatcherQueueController" => Windows.System.DispatcherQueueController.FromAbi(thisPtr),
            _ => MarshalNativeToManagedSlow(thisPtr),
        };
    }

    private object MarshalNativeToManagedSlow(nint pNativeData)
    {
        if (!lookedForFromAbi)
        {
            Type? type = Type.GetType($"{className}, {AssemblyFullName}");

            fromAbiMethod = type?.GetMethod("FromAbi");
            lookedForFromAbi = true;
        }

        if (fromAbiMethod is not null)
        {
            return fromAbiMethod.Invoke(default, new object[] { pNativeData })!;
        }
        else
        {
            return Marshal.GetObjectForIUnknown(pNativeData);
        }
    }
}
