// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.AdvApi32;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Win32.Registry;

internal sealed partial class RegistryWatcher : IDisposable
{
    private const REG_SAM_FLAGS RegSamFlags =
        REG_SAM_FLAGS.KEY_QUERY_VALUE |
        REG_SAM_FLAGS.KEY_NOTIFY |
        REG_SAM_FLAGS.KEY_READ;

    private const REG_NOTIFY_FILTER RegNotifyFilters =
        REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_NAME |
        REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_ATTRIBUTES |
        REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_LAST_SET |
        REG_NOTIFY_FILTER.REG_NOTIFY_CHANGE_SECURITY;

    private readonly CancellationTokenSource cancellationTokenSource = new();

    private readonly HKEY hKey;
    private readonly string subKey;
    private readonly Action valueChangedCallback;
    private readonly Lock syncRoot = new();
    private bool disposed;

    public RegistryWatcher(string keyName, Action valueChangedCallback)
    {
        string[] pathArray = keyName.Split('\\');

        hKey = pathArray[0] switch
        {
            nameof(HKEY.HKEY_CLASSES_ROOT) => HKEY.HKEY_CLASSES_ROOT,
            nameof(HKEY.HKEY_CURRENT_USER) => HKEY.HKEY_CURRENT_USER,
            nameof(HKEY.HKEY_LOCAL_MACHINE) => HKEY.HKEY_LOCAL_MACHINE,
            nameof(HKEY.HKEY_USERS) => HKEY.HKEY_USERS,
            nameof(HKEY.HKEY_CURRENT_CONFIG) => HKEY.HKEY_CURRENT_CONFIG,
            _ => throw new ArgumentException($"The registry hive '{pathArray[0]}' is not supported", nameof(keyName)),
        };

        subKey = string.Join("\\", pathArray[1..]);
        this.valueChangedCallback = valueChangedCallback;
    }

    public void Start(ILogger logger)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        WatchAsync(cancellationTokenSource.Token).SafeForget(logger);
    }

    public void Dispose()
    {
        // Standard no-reentrancy pattern
        if (disposed)
        {
            return;
        }

        lock (syncRoot)
        {
            if (disposed)
            {
                return;
            }

            // Cancel the outer while loop
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();

            disposed = true;

            GC.SuppressFinalize(this);
        }
    }

    private async ValueTask WatchAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

            HRESULT hResult = HRESULT_FROM_WIN32(RegOpenKeyExW(hKey, subKey, 0, RegSamFlags, out HKEY registryKey));
            Marshal.ThrowExceptionForHR(hResult);

            using (AutoResetEvent notifyEvent = new(false))
            {
                HANDLE hEvent = notifyEvent.SafeWaitHandle.DangerousGetHandle();

                try
                {
                    HRESULT hRESULT = HRESULT_FROM_WIN32(RegNotifyChangeKeyValue(registryKey, true, RegNotifyFilters, hEvent, true));
                    Marshal.ThrowExceptionForHR(hRESULT);

                    if (WaitHandle.WaitAny([notifyEvent, token.WaitHandle]) is 0)
                    {
                        valueChangedCallback();
                    }
                }
                finally
                {
                    RegCloseKey(registryKey);
                }
            }
        }
    }
}