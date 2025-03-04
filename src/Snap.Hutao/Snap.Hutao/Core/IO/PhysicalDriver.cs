// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Storage.FileSystem;
using Snap.Hutao.Win32.System.Ioctl;
using System.IO;
using System.Runtime.InteropServices;
using Sentry.Protocol;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.Kernel32;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Core.IO;

internal static class PhysicalDriver
{
    /// <summary>
    /// Safely get the SSD information of the physical driver.
    /// </summary>
    /// <param name="path">path in a driver</param>
    /// <returns>
    /// <see langword="null"/> if any exception occurs,
    /// <see langword="true"/> if it's a SSD,
    /// otherwise <see langword="false"/>
    /// </returns>
    public static bool? GetIsSolidState(string path)
    {
        try
        {
            return DangerousGetIsSolidState(path);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return null;
        }
    }

    // From Microsoft.VisualStudio.Setup.Services.DiskInfo
    // Check if the driver is trim enabled and not incurs seek penalty.
    // https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew#physical-disks-and-volumes
    public static bool DangerousGetIsSolidState(string path)
    {
        if (LocalSetting.Get(SettingKeys.OverridePhysicalDriverType, false))
        {
            return LocalSetting.Get(SettingKeys.PhysicalDriverIsAlwaysSolidState, false);
        }

        string? root = Path.GetPathRoot(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(root, "The path does not contain a root.");

        GetDeviceNumber($@"\\.\{root[..^1]}", out uint deviceNumber);
        GetIsSsd($@"\\.\PHYSICALDRIVE{deviceNumber}", out bool isSsd);

        return isSsd;
    }

    private static unsafe void GetDeviceNumber(string fileName, out uint deviceNumber)
    {
        HANDLE hLogicalDriver = default;
        try
        {
            hLogicalDriver = CreateDirectAccessStorageDeviceHandle(fileName);
            STORAGE_DEVICE_NUMBER number = default;
            if (DeviceIoControl(hLogicalDriver, IOCTL_STORAGE_GET_DEVICE_NUMBER, default, default, &number, (uint)sizeof(STORAGE_DEVICE_NUMBER), default, default))
            {
                deviceNumber = number.DeviceNumber;
                return;
            }

            WIN32_ERROR error = GetLastError();
            if (error is not WIN32_ERROR.ERROR_INVALID_FUNCTION)
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(error));
            }

            // This logical driver belongs to a partitionable device.
            Span<byte> buffer = stackalloc byte[sizeof(VOLUME_DISK_EXTENTS) + (sizeof(DISK_EXTENT) * 1)];
            if (DeviceIoControl(hLogicalDriver, IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, default, default, buffer, default, default))
            {
                deviceNumber = MemoryMarshal.AsRef<VOLUME_DISK_EXTENTS>(buffer).Extents[0].DiskNumber;
                return;
            }

            WIN32_ERROR error2 = GetLastError();
            if (error2 is not WIN32_ERROR.ERROR_MORE_DATA)
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(error2));
            }

            // The volume has multiple extents.
            buffer = stackalloc byte[sizeof(VOLUME_DISK_EXTENTS) + (sizeof(DISK_EXTENT) * (int)MemoryMarshal.AsRef<VOLUME_DISK_EXTENTS>(buffer).NumberOfDiskExtents)];
            if (DeviceIoControl(hLogicalDriver, IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS, default, default, buffer, default, default))
            {
                deviceNumber = MemoryMarshal.AsRef<VOLUME_DISK_EXTENTS>(buffer).Extents[0].DiskNumber;
                return;
            }

            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            throw HutaoException.Throw("Failed to get the device number.");
        }
        finally
        {
            CloseHandle(hLogicalDriver);
        }
    }

    private static unsafe void GetIsSsd(string fileName, out bool isSsd)
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
                isSsd = false;
                return;
            }

            query.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceProperty;
            STORAGE_DEVICE_DESCRIPTOR device = default;
            if (!DeviceIoControl(hPhysicalDriver, IOCTL_STORAGE_QUERY_PROPERTY, &query, (uint)sizeof(STORAGE_PROPERTY_QUERY), &device, (uint)sizeof(STORAGE_DEVICE_DESCRIPTOR), default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }

            if (device.BusType is STORAGE_BUS_TYPE.BusTypeUsb)
            {
                isSsd = false;
                return;
            }

            query.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceSeekPenaltyProperty;
            DEVICE_SEEK_PENALTY_DESCRIPTOR seekPenalty = default;
            if (!DeviceIoControl(hPhysicalDriver, IOCTL_STORAGE_QUERY_PROPERTY, &query, (uint)sizeof(STORAGE_PROPERTY_QUERY), &seekPenalty, (uint)sizeof(DEVICE_SEEK_PENALTY_DESCRIPTOR), default, default))
            {
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(GetLastError()));
            }

            isSsd = !seekPenalty.IncursSeekPenalty;
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