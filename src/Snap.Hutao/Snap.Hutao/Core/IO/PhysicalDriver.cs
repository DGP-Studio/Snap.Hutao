// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Storage.FileSystem;
using Snap.Hutao.Win32.System.Ioctl;
using System.IO;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Core.IO;

internal static class PhysicalDriver
{
    // From Microsoft.VisualStudio.Setup.Services.DiskInfo
    // Check if the driver is trim enabled and not incurs seek penalty.
    // https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew#physical-disks-and-volumes
    public static unsafe bool DangerousGetIsSolidState(string path)
    {
        if (LocalSetting.Get(SettingKeys.OverridePhysicalDriverType, false))
        {
            return LocalSetting.Get(SettingKeys.PhysicalDriverIsAlwaysSolidState, false);
        }

        string? root = Path.GetPathRoot(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(root, "The path does not contain a root.");

        GetDeviceNumber($@"\\.\{root[..^1]}", out uint deviceNumber);
        GetIsSSD($@"\\.\PHYSICALDRIVE{deviceNumber}", out bool isSSD);

        return isSSD;
    }

    private static unsafe void GetDeviceNumber(string fileName, out uint deviceNumber)
    {
        HANDLE hLogicalDriver = default;
        try
        {
            hLogicalDriver = CreateDirectAccessStorageDeviceHandle(fileName);
            STORAGE_DEVICE_NUMBER number = default;
            if (!DeviceIoControl(hLogicalDriver, IOCTL_STORAGE_GET_DEVICE_NUMBER, default, default, &number, (uint)sizeof(STORAGE_DEVICE_NUMBER), default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }

            deviceNumber = number.DeviceNumber;
        }
        finally
        {
            CloseHandle(hLogicalDriver);
        }
    }

    private static unsafe void GetIsSSD(string fileName, out bool isSSD)
    {
        HANDLE hPhysicalDriver = default;
        try
        {
            hPhysicalDriver = CreateDirectAccessStorageDeviceHandle(fileName);

            STORAGE_PROPERTY_QUERY query = default;
            query.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceTrimProperty;
            query.QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery;
            DEVICE_TRIM_DESCRIPTOR deviceTrim = default;
            if (!DeviceIoControl(hPhysicalDriver, IOCTL_STORAGE_QUERY_PROPERTY, &query, (uint)sizeof(STORAGE_PROPERTY_QUERY), &deviceTrim, (uint)sizeof(DEVICE_TRIM_DESCRIPTOR), default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }

            if (!deviceTrim.TrimEnabled)
            {
                isSSD = false;
                return;
            }

            query.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceSeekPenaltyProperty;
            DEVICE_SEEK_PENALTY_DESCRIPTOR seekPenalty = default;
            if (!DeviceIoControl(hPhysicalDriver, IOCTL_STORAGE_QUERY_PROPERTY, &query, (uint)sizeof(STORAGE_PROPERTY_QUERY), &seekPenalty, (uint)sizeof(DEVICE_SEEK_PENALTY_DESCRIPTOR), default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }

            isSSD = !seekPenalty.IncursSeekPenalty;
        }
        finally
        {
            CloseHandle(hPhysicalDriver);
        }
    }

    private static unsafe HANDLE CreateDirectAccessStorageDeviceHandle(string fileName)
    {
        fixed (char* lpFileName = fileName)
        {
            HANDLE hDevice = CreateFileW(lpFileName, 0x80000000, FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE, default, FILE_CREATION_DISPOSITION.OPEN_EXISTING, FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL, default);
            if (hDevice == HANDLE.INVALID_HANDLE_VALUE)
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }

            return hDevice;
        }
    }
}