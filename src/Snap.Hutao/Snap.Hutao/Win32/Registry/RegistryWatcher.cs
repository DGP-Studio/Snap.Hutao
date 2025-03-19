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

    private readonly CancellationTokenSource cts = new();

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

            // ReSharper disable once LocalizableElement
            _ => throw new ArgumentException($"The registry hive '{pathArray[0]}' is not supported", nameof(keyName)),
        };

        subKey = string.Join("\\", pathArray[1..]);
        this.valueChangedCallback = valueChangedCallback;
    }

    public void Start()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        ThreadPool.QueueUserWorkItem(Watch, cts);
    }

    public void Dispose()
    {
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
            cts.Cancel();
            cts.Dispose();

            disposed = true;
        }
    }

    private void Watch(object? state)
    {
        if (state is not CancellationTokenSource cts)
        {
            return;
        }

        try
        {
            CancellationToken token = cts.Token;

            while (!token.IsCancellationRequested)
            {
                HRESULT hResult = HRESULT_FROM_WIN32(RegOpenKeyExW(hKey, subKey, 0, RegSamFlags, out HKEY registryKey));
                Marshal.ThrowExceptionForHR(hResult);

                using (AutoResetEvent notifyEvent = new(false))
                {
                    try
                    {
                        HRESULT hr = HRESULT_FROM_WIN32(RegNotifyChangeKeyValue(registryKey, true, RegNotifyFilters, notifyEvent.SafeWaitHandle.DangerousGetHandle(), true));
                        Marshal.ThrowExceptionForHR(hr);

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
        catch (ObjectDisposedException)
        {
            // Watcher was disposed, exit gracefully
        }
    }
}