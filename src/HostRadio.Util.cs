using System;
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
    private static uint CTL_CODE(uint deviceType, uint function, uint method, FILE_ACCESS_RIGHTS access)
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

        using var hFind = PInvoke.BluetoothFindFirstDevice(searchParams, ref deviceInfo);

        if (hFind.IsInvalid) return false;

        byte[] targetAddress = address.GetAddressBytes();
        if (targetAddress.Length != 6)
        {
            return false;
        }

        Span<byte> reversed = stackalloc byte[6];
        for (int i = 0; i < 6; i++)
        {
            reversed[i] = targetAddress[5 - i];
        }

        do
        {
            if (deviceInfo.Address.Anonymous.rgBytes.Equals(reversed))
            {
                return true;
            }
        } while (PInvoke.BluetoothFindNextDevice(hFind, ref deviceInfo));

        return false;
    }

    private static unsafe void AdjustProcessPrivileges()
    {
        var processToken = HANDLE.Null;

        try
        {
            var result = PInvoke.OpenProcessToken(
                PInvoke.GetCurrentProcess(),
                TOKEN_ACCESS_MASK.TOKEN_ADJUST_PRIVILEGES | TOKEN_ACCESS_MASK.TOKEN_QUERY,
                &processToken
            );

            if (!result)
                throw new AdjustProcessPrivilegesException("OpenProcessToken call failed.",
                    (uint)Marshal.GetLastWin32Error());

            result = PInvoke.LookupPrivilegeValue(null, "SeLoadDriverPrivilege", out var luid);

            if (!result)
                throw new AdjustProcessPrivilegesException("LookupPrivilegeValue call failed.",
                    (uint)Marshal.GetLastWin32Error());

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

            var error = (WIN32_ERROR)Marshal.GetLastWin32Error();

            if (error == WIN32_ERROR.ERROR_NOT_ALL_ASSIGNED)
                throw new AdjustProcessPrivilegesException(
                    "AdjustTokenPrivileges failed. Make sure to run the current process with elevated privileges.",
                    (uint)error);

            if (error != WIN32_ERROR.ERROR_SUCCESS)
                throw new AdjustProcessPrivilegesException("AdjustTokenPrivileges failed.",
                    (uint)error);
        }
        finally
        {
            if (processToken != HANDLE.Null) PInvoke.CloseHandle(processToken);
        }
    }
}