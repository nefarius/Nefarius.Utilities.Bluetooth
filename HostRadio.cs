using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Devices.Bluetooth;
using Windows.Win32.Foundation;

using JetBrains.Annotations;

using Microsoft.Win32.SafeHandles;

namespace Nefarius.Utilities.Bluetooth;

public class HostRadioException : Exception
{
    internal HostRadioException(string message) : base(message)
    {
    }

    internal HostRadioException(string message, uint errorCode) : this(message)
    {
        NativeErrorCode = errorCode;
    }

    public uint NativeErrorCode { get; }
}

/// <summary>
///     Represents a Bluetooth Host Radio.
/// </summary>
public class HostRadio : IDisposable
{
    public static readonly Guid HumanInterfaceDeviceServiceClassUuid =
        Guid.Parse("{0x1124,0x0000,0x1000,{0x80,0x00,0x00,0x80,0x5F,0x9B,0x34,0xFB}}");

    private readonly SafeFileHandle _radioHandle;

    public HostRadio()
    {
        BLUETOOTH_FIND_RADIO_PARAMS radioParams;
        radioParams.dwSize = (uint)Marshal.SizeOf<BLUETOOTH_FIND_RADIO_PARAMS>();

        nint hFind = PInvoke.BluetoothFindFirstRadio(radioParams, out _radioHandle);

        if (hFind == 0)
        {
            throw new HostRadioException("Bluetooth host radio not found.", (uint)Marshal.GetLastWin32Error());
        }

        PInvoke.BluetoothFindRadioClose(hFind);
    }

    public void Dispose()
    {
        _radioHandle.Dispose();
    }

    [UsedImplicitly]
    public void SetServiceStateForDevice(PhysicalAddress address, Guid serviceGuid, bool enabled)
    {
        bool found = FindDeviceByAddress(address, out BLUETOOTH_DEVICE_INFO_STRUCT deviceInfo);

        if (!found)
        {
            return;
        }

        uint flags = enabled ? PInvoke.BLUETOOTH_SERVICE_ENABLE : PInvoke.BLUETOOTH_SERVICE_DISABLE;

        uint error = PInvoke.BluetoothSetServiceState(_radioHandle, deviceInfo, serviceGuid, flags);

        if (error != (uint)WIN32_ERROR.ERROR_SUCCESS)
        {
            throw new HostRadioException("Failed to set service state.", error);
        }
    }

    [UsedImplicitly]
    public unsafe bool GetServiceStateForDevice(PhysicalAddress address, Guid serviceGuid, out bool enabled)
    {
        enabled = false;

        bool found = FindDeviceByAddress(address, out BLUETOOTH_DEVICE_INFO_STRUCT deviceInfo);

        if (!found)
        {
            return false;
        }

        uint numServices = 0;
        uint error = PInvoke.BluetoothEnumerateInstalledServices(_radioHandle, deviceInfo, ref numServices, null);

        if ((error == (uint)WIN32_ERROR.ERROR_SUCCESS && numServices == 0))
        {
            enabled = false;
            return true;
        }

        if (error != (uint)WIN32_ERROR.ERROR_SUCCESS && error != (uint)WIN32_ERROR.ERROR_MORE_DATA)
        {
            throw new HostRadioException("Failed to enumerate services.", error);
        }

        Guid* services = stackalloc Guid[(int)numServices];
        error = PInvoke.BluetoothEnumerateInstalledServices(_radioHandle, deviceInfo, ref numServices, services);

        if (error != (uint)WIN32_ERROR.ERROR_SUCCESS)
        {
            throw new HostRadioException("Failed to enumerate services.", error);
        }

        for (int i = 0; i < numServices; i++)
        {
            Guid uuid = services[i];

            if (uuid != serviceGuid)
            {
                continue;
            }

            enabled = true;
            return true;
        }

        return false;
    }

    private bool FindDeviceByAddress(PhysicalAddress address, out BLUETOOTH_DEVICE_INFO_STRUCT deviceInfo)
    {
        BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams = new()
        {
            dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_SEARCH_PARAMS>(),
            fReturnAuthenticated = true,
            fReturnConnected = true,
            fReturnRemembered = true,
            fReturnUnknown = true,
            hRadio = new HANDLE(_radioHandle.DangerousGetHandle())
        };

        deviceInfo = new BLUETOOTH_DEVICE_INFO_STRUCT { dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_INFO_STRUCT>() };

        nint hFind = PInvoke.BluetoothFindFirstDevice(searchParams, ref deviceInfo);

        if (hFind == 0)
        {
            return false;
        }

        try
        {
            do
            {
                if (deviceInfo.Address.Anonymous.rgBytes.Equals(address.GetAddressBytes().Reverse().ToArray().AsSpan()))
                {
                    return true;
                }
            } while (PInvoke.BluetoothFindNextDevice(hFind, ref deviceInfo));
        }
        finally
        {
            PInvoke.BluetoothFindDeviceClose(hFind);
        }

        return false;
    }
}