﻿using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Devices.Bluetooth;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.Storage.FileSystem;

using Nefarius.Utilities.Bluetooth.Exceptions;
using Nefarius.Utilities.Bluetooth.Util;

namespace Nefarius.Utilities.Bluetooth;

public sealed partial class HostRadio
{
    private static UInt32 CTL_CODE(uint deviceType, uint function, uint method, FILE_ACCESS_RIGHTS access)
    {
        return (deviceType << 16) | ((uint)access << 14) | (function << 2) | method;
    }

    private bool FindDeviceByAddress(PhysicalAddress address, out BLUETOOTH_DEVICE_INFO deviceInfo)
    {
        BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams = new()
        {
            dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_SEARCH_PARAMS>(),
            fReturnAuthenticated = true,
            fReturnConnected = true,
            fReturnRemembered = true,
            fReturnUnknown = true,
            hRadio = _radioHandle.ToHandle()
        };

        deviceInfo = new BLUETOOTH_DEVICE_INFO { dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_INFO>() };

        using BluetoothFindDeviceCloseSafeHandle hFind = PInvoke.BluetoothFindFirstDevice(searchParams, ref deviceInfo);

        if (hFind.IsInvalid)
        {
            return false;
        }

        do
        {
            if (deviceInfo.Address.Anonymous.rgBytes.Equals(address.GetAddressBytes().Reverse().ToArray().AsSpan()))
            {
                return true;
            }
        } while (PInvoke.BluetoothFindNextDevice(hFind, ref deviceInfo));

        return false;
    }

    private static unsafe void AdjustProcessPrivileges()
    {
        HANDLE processToken = HANDLE.Null;

        try
        {
            BOOL result = PInvoke.OpenProcessToken(
                PInvoke.GetCurrentProcess(),
                TOKEN_ACCESS_MASK.TOKEN_ADJUST_PRIVILEGES | TOKEN_ACCESS_MASK.TOKEN_QUERY,
                &processToken
            );

            if (!result)
            {
                throw new AdjustProcessPrivilegesException("OpenProcessToken call failed.",
                    (uint)Marshal.GetLastWin32Error());
            }

            result = PInvoke.LookupPrivilegeValue(null, "SeLoadDriverPrivilege", out LUID luid);

            if (!result)
            {
                throw new AdjustProcessPrivilegesException("LookupPrivilegeValue call failed.",
                    (uint)Marshal.GetLastWin32Error());
            }

            TOKEN_PRIVILEGES tp = new() { PrivilegeCount = 1 };
            tp.Privileges[0].Luid = luid;
            tp.Privileges[0].Attributes = TOKEN_PRIVILEGES_ATTRIBUTES.SE_PRIVILEGE_ENABLED;

            PInvoke.AdjustTokenPrivileges(
                processToken,
                new BOOL(false),
                &tp,
                (uint)Marshal.SizeOf<TOKEN_PRIVILEGES>(),
                null,
                null
            );

            WIN32_ERROR error = (WIN32_ERROR)Marshal.GetLastWin32Error();

            if (error == WIN32_ERROR.ERROR_NOT_ALL_ASSIGNED)
            {
                throw new AdjustProcessPrivilegesException(
                    "AdjustTokenPrivileges failed. Make sure to run the current process with elevated privileges.",
                    (uint)error);
            }

            if (error != WIN32_ERROR.ERROR_SUCCESS)
            {
                throw new AdjustProcessPrivilegesException("AdjustTokenPrivileges failed.",
                    (uint)error);
            }
        }
        finally
        {
            if (processToken != HANDLE.Null)
            {
                PInvoke.CloseHandle(processToken);
            }
        }
    }
}