// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
        string? root = Path.GetPathRoot(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(root, "The path does not contain a root.");

        HANDLE hLogicalDriver = default;
        STORAGE_DEVICE_NUMBER number = default;
        try
        {
            string logicalDriverName = $@"\\.\{root[..^1]}";
            fixed (char* lpFileName = logicalDriverName)
            {
                // winnt.h
                // GENERIC_READ = 0x80000000L
                hLogicalDriver = CreateFileW(lpFileName, 0x80000000, FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE, default, FILE_CREATION_DISPOSITION.OPEN_EXISTING, default, default);
                if (hLogicalDriver == HANDLE.INVALID_HANDLE_VALUE)
                {
                    Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
                }
            }

            if (!DeviceIoControl(hLogicalDriver, IOCTL_STORAGE_GET_DEVICE_NUMBER, default, default, &number, (uint)sizeof(STORAGE_DEVICE_NUMBER), default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }
        }
        finally
        {
            CloseHandle(hLogicalDriver);
        }

        HANDLE hPhysicalDriver = default;
        try
        {
            string physicalDriverName = $@"\\.\PHYSICALDRIVE{number.DeviceNumber}";
            fixed (char* lpFileName = physicalDriverName)
            {
                // winnt.h
                // GENERIC_READ = 0x80000000L
                hPhysicalDriver = CreateFileW(lpFileName, 0x80000000, FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE, default, FILE_CREATION_DISPOSITION.OPEN_EXISTING, FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL, default);
                if (hPhysicalDriver == HANDLE.INVALID_HANDLE_VALUE)
                {
                    Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
                }
            }

            STORAGE_PROPERTY_QUERY query = default;
            query.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceTrimProperty;
            query.QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery;
            DEVICE_TRIM_DESCRIPTOR descriptor = default;
            if (!DeviceIoControl(hPhysicalDriver, IOCTL_STORAGE_QUERY_PROPERTY, &query, (uint)sizeof(STORAGE_PROPERTY_QUERY), &descriptor, (uint)sizeof(DEVICE_TRIM_DESCRIPTOR), default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }

            if (!descriptor.TrimEnabled)
            {
                return false;
            }

            query.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceSeekPenaltyProperty;
            DEVICE_SEEK_PENALTY_DESCRIPTOR seekPenalty = default;
            if (!DeviceIoControl(hPhysicalDriver, IOCTL_STORAGE_QUERY_PROPERTY, &query, (uint)sizeof(STORAGE_PROPERTY_QUERY), &seekPenalty, (uint)sizeof(DEVICE_SEEK_PENALTY_DESCRIPTOR), default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }

            return !seekPenalty.IncursSeekPenalty;
        }
        finally
        {
            CloseHandle(hPhysicalDriver);
        }
    }
}