using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Devices.Bluetooth;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

using JetBrains.Annotations;

using Microsoft.Win32.SafeHandles;

using Nefarius.Utilities.DeviceManagement.PnP;

namespace Nefarius.Utilities.Bluetooth;

public class HostRadioException : Exception
{
    [UsedImplicitly]
    internal HostRadioException(string message) : base(message)
    {
    }

    internal HostRadioException(string message, uint errorCode) : this(message)
    {
        NativeErrorCode = errorCode;
    }

    [UsedImplicitly]
    public uint NativeErrorCode { get; }
}

/// <summary>
///     Represents a Bluetooth Host Radio.
/// </summary>
public class HostRadio : IDisposable
{
    private readonly SafeFileHandle _radioHandle;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="autoEnable">True to automatically enable the radio if currently disabled, false will throw an exception.</param>
    /// <exception cref="HostRadioException">Radio handle access has failed.</exception>
    public HostRadio(bool autoEnable = true)
    {
        BLUETOOTH_FIND_RADIO_PARAMS radioParams;
        radioParams.dwSize = (uint)Marshal.SizeOf<BLUETOOTH_FIND_RADIO_PARAMS>();

        nint hFind = PInvoke.BluetoothFindFirstRadio(radioParams, out _radioHandle);

        if (hFind == 0)
        {
            // radio might be disabled, check if interface is exposed to get device handle
            if (!Devcon.FindByInterfaceGuid(DeviceInterface, out string path, out _))
            {
                throw new HostRadioException("Bluetooth host radio not found.", (uint)Marshal.GetLastWin32Error());
            }

            if (!autoEnable)
            {
                throw new HostRadioException("Bluetooth host radio found but disabled.",
                    (uint)Marshal.GetLastWin32Error());
            }

            // open handle the old-fashioned way
            _radioHandle = PInvoke.CreateFile(
                path,
                FILE_ACCESS_FLAGS.FILE_GENERIC_READ | FILE_ACCESS_FLAGS.FILE_GENERIC_WRITE,
                FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                null,
                FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                null
            );

            if (_radioHandle.IsInvalid)
            {
                throw new HostRadioException("Bluetooth host radio found but handle access failed.",
                    (uint)Marshal.GetLastWin32Error());
            }

            // send packet that enables all device functions
            EnableRadio();
        }
        else
        {
            PInvoke.BluetoothFindRadioClose(hFind);
        }
    }

    [UsedImplicitly]
    public static Guid HumanInterfaceDeviceServiceClassUuid =>
        Guid.Parse("{0x1124,0x0000,0x1000,{0x80,0x00,0x00,0x80,0x5F,0x9B,0x34,0xFB}}");

    /// <summary>
    ///     Device Interface GUID.
    /// </summary>
    [UsedImplicitly]
    public static Guid DeviceInterface => Guid.Parse("{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}");

    public void Dispose()
    {
        _radioHandle.Dispose();
    }

    /// <summary>
    ///     Disables the host radio.
    /// </summary>
    /// <exception cref="HostRadioException"></exception>
    [UsedImplicitly]
    public unsafe void DisableRadio()
    {
        byte[] payload = { 0x04, 0x00, 0x00, 0x00 };

        fixed (byte* ptr = payload)
        {
            BOOL ret = PInvoke.DeviceIoControl(
                _radioHandle,
                0x411184,
                ptr,
                4,
                null,
                0,
                null,
                null
            );

            if (!ret)
            {
                throw new HostRadioException("Failed to disable host radio.", (uint)Marshal.GetLastWin32Error());
            }
        }
    }

    /// <summary>
    ///     Enables the host radio.
    /// </summary>
    /// <exception cref="HostRadioException"></exception>
    [UsedImplicitly]
    public unsafe void EnableRadio()
    {
        byte[] payload = { 0x02, 0x00, 0x00, 0x00 };

        fixed (byte* ptr = payload)
        {
            BOOL ret = PInvoke.DeviceIoControl(
                _radioHandle,
                0x411184,
                ptr,
                4,
                null,
                0,
                null,
                null
            );

            if (!ret)
            {
                throw new HostRadioException("Failed to enable host radio.", (uint)Marshal.GetLastWin32Error());
            }
        }
    }

    /// <summary>
    ///     Restarts the host radio.
    /// </summary>
    public void RestartRadio()
    {
        DisableRadio();
        EnableRadio();
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

        if (error == (uint)WIN32_ERROR.ERROR_SUCCESS && numServices == 0)
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