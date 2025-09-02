// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CodeFixes;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Factory.Process;

internal sealed class ProcessFactory
{
    public static void KillCurrent()
    {
        global::System.Diagnostics.Process.GetCurrentProcess().Kill();
    }

    public static bool TryGetById(int processId, [NotNullWhen(true)] out IProcess? process)
    {
        try
        {
            process = new DiagnosticsProcess(global::System.Diagnostics.Process.GetProcessById(processId));
            return true;
        }
        catch (Exception ex)
        {
            // Process with an Id of $id$ is not running.
            if (ex is not ArgumentException)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        process = null;
        return false;
    }

    public static bool IsRunning(string processName, string mainWindowTitle)
    {
        int currentSessionId = global::System.Diagnostics.Process.GetCurrentProcess().SessionId;
        global::System.Diagnostics.Process[] processes = global::System.Diagnostics.Process.GetProcessesByName(processName);

        if (processes.Length <= 0)
        {
            return false;
        }

        foreach (ref readonly global::System.Diagnostics.Process process in processes.AsSpan())
        {
            try
            {
                if (process.SessionId != currentSessionId)
                {
                    continue;
                }

                if (string.Equals(process.MainWindowTitle, mainWindowTitle, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // Force access handle to check whether process has exited
                _ = process.Handle;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                break;
            }
        }

        return false;
    }

    public static bool IsRunning(ReadOnlySpan<string> processNames, [NotNullWhen(true)] out IProcess? runningProcess)
    {
        int currentSessionId = global::System.Diagnostics.Process.GetCurrentProcess().SessionId;
        global::System.Diagnostics.Process[] processes = global::System.Diagnostics.Process.GetProcesses();

        // GetProcesses once and manually loop is O(n)
        foreach (ref readonly global::System.Diagnostics.Process process in processes.AsSpan())
        {
            try
            {
                if (process.SessionId != currentSessionId)
                {
                    continue;
                }

                if (!process.ProcessName.EqualsAny(processNames, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Force access handle to check whether process has exited
                _ = process.Handle;

                runningProcess = new DiagnosticsProcess(process);
                return true;
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    // 拒绝访问。
                    case Win32Exception we:
                        if (we.NativeErrorCode is (int)WIN32_ERROR.ERROR_ACCESS_DENIED)
                        {
                            runningProcess = new DiagnosticsProcess(process);
                            return true;
                        }

                        break;

                    // Cannot process request because the process ($id$) has exited.
                    case InvalidOperationException ioe:
                        runningProcess = default;
                        return false;
                }

                SentrySdk.CaptureException(ex);
                break;
            }
        }

        runningProcess = default;
        return false;
    }

    public static IProcess CreateUsingShellExecuteRunAs(string arguments, string fileName, string workingDirectory)
    {
        global::System.Diagnostics.Process process = new()
        {
            StartInfo = new()
            {
                Arguments = arguments,
                FileName = fileName,
                UseShellExecute = true,
                Verb = "runas",
                WorkingDirectory = workingDirectory,
            },
        };

        return new DiagnosticsProcess(process);
    }

    public static unsafe IProcess CreateSuspended(string arguments, string fileName, string workingDirectory)
    {
        fixed (char* pArguments = arguments)
        {
            fixed (char* pFileName = fileName)
            {
                fixed (char* pWorkingDirectory = workingDirectory)
                {
                    HutaoNativeProcessStartInfo startInfo = new()
                    {
                        ApplicationName = pFileName,
                        CommandLine = pArguments,
                        InheritHandles = BOOL.FALSE,
                        CreationFlags = Win32.System.Threading.PROCESS_CREATION_FLAGS.CREATE_SUSPENDED,
                        CurrentDirectory = pWorkingDirectory,
                    };

                    return new NativeProcess(HutaoNative.Instance.MakeProcess(startInfo));
                }
            }
        }
    }

    public static void StartUsingShellExecute(string arguments, string fileName)
    {
        global::System.Diagnostics.Process.Start(new global::System.Diagnostics.ProcessStartInfo
        {
            Arguments = arguments,
            FileName = fileName,
            UseShellExecute = true,
        });
    }

    public static void StartUsingShellExecuteRunAs(string fileName)
    {
        global::System.Diagnostics.Process.Start(new global::System.Diagnostics.ProcessStartInfo
        {
            FileName = fileName,
            UseShellExecute = true,
            Verb = "runas",
        });
    }
}