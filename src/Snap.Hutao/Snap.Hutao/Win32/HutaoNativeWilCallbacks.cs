// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Snap.Hutao.Win32;

internal static unsafe class HutaoNativeWilCallbacks
{
    public static void HutaoInitializeWilCallbacks()
    {
        Marshal.ThrowExceptionForHR(HutaoInitializeWilCallbacks(&WilLoggingImpl, &WilMessageImpl));
    }

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport(HutaoNativeMethods.DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT HutaoInitializeWilCallbacks(delegate* unmanaged[Stdcall]<FailureInfo*, void> loggingCallback, delegate* unmanaged[Stdcall]<FailureInfo*, PWSTR, ulong, void> messageCallback);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void WilLoggingImpl(FailureInfo* failure)
    {
        HutaoNativeException exception = new(*failure);
        if (string.IsNullOrWhiteSpace(exception.Message))
        {
            return;
        }

        SentrySdk.CaptureException(exception);
        SentrySdk.Flush();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void WilMessageImpl(FailureInfo* failure, PWSTR pszDebugMessage, ulong cchDebugMessage)
    {
        Debug.WriteLine(new ReadOnlySpan<char>(pszDebugMessage, (int)cchDebugMessage).ToString());
    }

#pragma warning disable CS0649
#pragma warning disable SA1201
#pragma warning disable SA1307

    // ReSharper disable InconsistentNaming
    internal enum FailureType
    {
        // THROW_...
        Exception,

        // RETURN_..._LOG or RETURN_..._MSG
        Return,

        // LOG_...
        Log,

        // FAIL_FAST_...
        FailFast,
    }

    internal enum FailureFlags
    {
        None = 0x00,
        RequestFailFast = 0x01,
        RequestSuppressTelemetry = 0x02,
        RequestDebugBreak = 0x04,
        NtStatus = 0x08,
    }

    internal struct CallContextInfo
    {
        // incrementing ID for this call context (unique across an individual module load within process)
        public long contextId;

        // the explicit name given to this context
        public PCSTR contextName;

        // [optional] Message that can be associated with the call context
        public PCWSTR contextMessage;
    }

    internal struct FailureInfo
    {
        public FailureType type;
        public FailureFlags flags;
        public HRESULT hr;
        public NTSTATUS status;

        // incrementing ID for this specific failure (unique across an individual module load within process)
        public long failureId;

        // Message is only present for _MSG logging (it's the Sprintf message)
        public PCWSTR pszMessage;

        // the thread this failure was originally encountered on
        public uint threadId;

        // [debug only] Capture code from the macro
        public PCSTR pszCode;

        // [debug only] The function name
        public PCSTR pszFunction;
        public PCSTR pszFile;
        public uint ulineNumber;

        // How many failures of 'type' have been reported in this module so far
        public int cFailureCount;

        // General breakdown of the call context stack that generated this failure
        public PCSTR pszCallContext;

        // The outermost (first seen) call context
        public CallContextInfo callContextOriginating;

        // The most recently seen call context
        public CallContextInfo callContextCurrent;

        // The module where the failure originated
        public PCSTR pszModule;

        // The return address to the point that called the macro
        public nint returnAddress;

        // The return address of the function that includes the macro
        public nint callerReturnAddress;
    }

    // ReSharper restore InconsistentNaming
#pragma warning restore SA1307
#pragma warning restore SA1201
#pragma warning restore CS0649

    internal sealed class HutaoNativeException : Exception
    {
        public HutaoNativeException(FailureInfo info)
            : base($"{info.hr}: {MemoryMarshal.CreateReadOnlySpanFromNullTerminated(info.pszMessage).ToString()}")
        {
            Data["Type"] = info.type;
            Data["Flags"] = info.flags;
            HResult = info.hr;
            Data["Code"] = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(info.pszCode)); // Debug only
            Data["Module"] = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(info.pszModule));
            Data["ReturnAddress"] = $"0x{info.returnAddress:X16}";
            Data["CallerReturnAddress"] = $"0x{info.callerReturnAddress:X16}";
            Data["FileAndLineNumber"] = $"{Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(info.pszFile))}:line {info.ulineNumber}";
        }
    }
}