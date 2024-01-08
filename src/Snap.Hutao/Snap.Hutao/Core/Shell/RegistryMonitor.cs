// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Registry;

namespace Snap.Hutao.Core.Shell;

internal sealed partial class RegistryMonitor : IDisposable
{
    private readonly ManualResetEvent eventTerminate = new(false);
    private readonly CancellationTokenSource cancellationTokenSource = new();

    private readonly REG_SAM_FLAGS samFlags = REG_SAM_FLAGS.KEY_QUERY_VALUE | REG_SAM_FLAGS.KEY_NOTIFY | REG_SAM_FLAGS.KEY_READ;
    private readonly REG_NOTIFY_FILTER notiftFilters = REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_NAME | REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_ATTRIBUTES |
                                              REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_LAST_SET | REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_SECURITY;

    private HKEY hKey;
    private string subKey = default!;
    private bool disposed;

    public RegistryMonitor(string name, EventHandler eventHandler)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        InitRegistryKey(name);
        RegChanged += eventHandler;
    }

    public event EventHandler? RegChanged;

    public static RegistryMonitor Create(string name, EventHandler eventHandler)
    {
        return new RegistryMonitor(name, eventHandler);
    }

    public void Start()
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        eventTerminate.Reset();
        MonitorCoreAsync(cancellationTokenSource.Token).SafeForget();
    }

    public void Dispose()
    {
        eventTerminate.Dispose();
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        disposed = true;
        GC.SuppressFinalize(this);
    }

    private void InitRegistryKey(string name)
    {
        string[] nameParts = name.Split('\\');

        switch (nameParts[0])
        {
            case "HKEY_CLASSES_ROOT":
            case "HKCR":
                hKey = HKEY.HKEY_CLASSES_ROOT;
                break;

            case "HKEY_CURRENT_USER":
            case "HKCU":
                hKey = HKEY.HKEY_CURRENT_USER;
                break;

            case "HKEY_LOCAL_MACHINE":
            case "HKLM":
                hKey = HKEY.HKEY_LOCAL_MACHINE;
                break;

            case "HKEY_USERS":
                hKey = HKEY.HKEY_USERS;
                break;

            case "HKEY_CURRENT_CONFIG":
                hKey = HKEY.HKEY_CURRENT_CONFIG;
                break;

            default:
                hKey = HKEY.Null;
                throw new ArgumentException("The registry hive '" + nameParts[0] + "' is not supported", nameof(name));
        }

        subKey = string.Join("\\", nameParts, 1, nameParts.Length - 1);
    }

    private async ValueTask MonitorCoreAsync(CancellationToken token)
    {
        using (PeriodicTimer timer = new(TimeSpan.FromSeconds(1)))
        {
            try
            {
                while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    WIN32_ERROR result = PInvoke.RegOpenKeyEx(hKey, subKey, 0, samFlags, out HKEY registryKey);
                    if (result != WIN32_ERROR.ERROR_SUCCESS)
                    {
                        throw new Win32Exception((int)result);
                    }

                    AutoResetEvent eventNotify = new(false);

                    try
                    {
                        WaitHandle[] waitHandles = [eventNotify, eventTerminate];
                        while (!eventTerminate.WaitOne(0, true))
                        {
                            result = PInvoke.RegNotifyChangeKeyValue(registryKey, true, notiftFilters, (HANDLE)eventNotify.SafeWaitHandle.DangerousGetHandle(), true);
                            if (result != 0)
                            {
                                throw new Win32Exception((int)result);
                            }

                            if (WaitHandle.WaitAny(waitHandles) == 0)
                            {
                                RegChanged?.Invoke(this, EventArgs.Empty);
                            }
                        }
                    }
                    finally
                    {
                        if (registryKey != IntPtr.Zero)
                        {
                            PInvoke.RegCloseKey(registryKey);
                        }

                        eventNotify.Dispose();
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
