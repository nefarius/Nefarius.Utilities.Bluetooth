using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Devices.Bluetooth;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;

using Microsoft.Win32.SafeHandles;

using Nefarius.Utilities.Bluetooth.Exceptions;
using Nefarius.Utilities.Bluetooth.Types;
using Nefarius.Utilities.DeviceManagement.PnP;

namespace Nefarius.Utilities.Bluetooth;

/// <summary>
///     Represents a Bluetooth Host Radio.
/// </summary>
/// <remarks>
///     Windows currently only supports one exclusive online Bluetooth host radio active at the same time. This class
///     can be extended in the future, should this limit ever get lifted.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed partial class HostRadio : IDisposable
{
    private static readonly uint IoctlSetRadioState = CTL_CODE(PInvoke.FILE_DEVICE_BLUETOOTH, 0x461,
        PInvoke.METHOD_BUFFERED, PInvoke.FILE_ANY_ACCESS);

    private static readonly uint IoctlBthDisconnectDevice = CTL_CODE(PInvoke.FILE_DEVICE_BLUETOOTH, 0x03,
        PInvoke.METHOD_BUFFERED, PInvoke.FILE_ANY_ACCESS);

    private static readonly uint IoctlBthSdpConnect = CTL_CODE(PInvoke.FILE_DEVICE_BLUETOOTH, 0x80,
        PInvoke.METHOD_BUFFERED, PInvoke.FILE_ANY_ACCESS);

    private static readonly uint IoctlBthSdpDisconnect = CTL_CODE(PInvoke.FILE_DEVICE_BLUETOOTH, 0x81,
        PInvoke.METHOD_BUFFERED, PInvoke.FILE_ANY_ACCESS);

    private static readonly uint IoctlHciCxn_ReadConnectedRssi = CTL_CODE(PInvoke.FILE_DEVICE_BLUETOOTH, 0x486,
        PInvoke.METHOD_BUFFERED, PInvoke.FILE_ANY_ACCESS);

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

            // should be accessible non-elevated without issues
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

    /// <summary>
    ///     HID Service Class GUID.
    /// </summary>
    public static Guid HumanInterfaceDeviceServiceClassUuid =>
        Guid.Parse("{0x1124,0x0000,0x1000,{0x80,0x00,0x00,0x80,0x5F,0x9B,0x34,0xFB}}");

    /// <summary>
    ///     Device Interface GUID.
    /// </summary>
    public static Guid DeviceInterface => Guid.Parse("{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}");

    /// <inheritdoc />
    public void Dispose()
    {
        _radioHandle.Dispose();
    }

    private static UInt32 CTL_CODE(uint deviceType, uint function, uint method, FILE_ACCESS_FLAGS access)
    {
        return (deviceType << 16) | ((uint)access << 14) | (function << 2) | method;
    }

    /// <summary>
    ///     Instruct host radio to disconnect a given remote device.
    /// </summary>
    /// <param name="address">A parseable MAC address string.</param>
    /// <remarks>
    ///     See
    ///     https://learn.microsoft.com/en-us/dotnet/api/system.net.networkinformation.physicaladdress.parse?view=net-7.0 for
    ///     valid string formats.
    /// </remarks>
    public void DisconnectRemoteDevice(string address)
    {
        DisconnectRemoteDevice(PhysicalAddress.Parse(address));
    }

    /// <summary>
    ///     Instruct host radio to disconnect a given remote device.
    /// </summary>
    /// <param name="device">The <see cref="RemoteDevice" /> to disconnect.</param>
    public void DisconnectRemoteDevice(RemoteDevice device)
    {
        DisconnectRemoteDevice(device.Address);
    }

    /// <summary>
    ///     Instruct host radio to disconnect a given remote device.
    /// </summary>
    /// <param name="device">The MAC address of the remote device.</param>
    public unsafe void DisconnectRemoteDevice(PhysicalAddress device)
    {
        int payloadSize = Marshal.SizeOf<ulong>();
        byte[] raw = new byte[] { 0x00, 0x00 }.Concat(device.GetAddressBytes()).Reverse().ToArray();
        long value = (long)BitConverter.ToUInt64(raw, 0);

        BOOL ret = PInvoke.DeviceIoControl(
            _radioHandle,
            IoctlBthDisconnectDevice,
            &value,
            (uint)payloadSize,
            null,
            0,
            null,
            null
        );

        if (!ret)
        {
            throw new HostRadioException("Failed to disconnect remote device.", (uint)Marshal.GetLastWin32Error());
        }
    }

    // TODO: finish testing and make public
    private unsafe void SdpConnect(PhysicalAddress device, out UInt64 handle)
    {
        byte[] raw = new byte[] { 0x00, 0x00 }.Concat(device.GetAddressBytes()).Reverse().ToArray();

        int payloadSize = Marshal.SizeOf<BTH_SDP_CONNECT>();
        BTH_SDP_CONNECT sdpConnect;
        sdpConnect.bthAddress = BitConverter.ToUInt64(raw, 0);
        sdpConnect.requestTimeout = (byte)PInvoke.SDP_REQUEST_TO_DEFAULT;
        sdpConnect.fSdpConnect = PInvoke.SDP_CONNECT_CACHE;

        BOOL ret = PInvoke.DeviceIoControl(
            _radioHandle,
            IoctlBthSdpConnect,
            &sdpConnect,
            (uint)payloadSize,
            &sdpConnect,
            (uint)payloadSize,
            null,
            null
        );

        if (!ret)
        {
            throw new HostRadioException("Failed to disconnect remote device.", (uint)Marshal.GetLastWin32Error());
        }

        handle = sdpConnect.hConnection;
    }

    // TODO: finish testing and make public
    private unsafe void SdpDisconnect(UInt64 handle)
    {
        int payloadSize = Marshal.SizeOf<UInt64>();

        BTH_SDP_DISCONNECT sdpDisconnect;
        sdpDisconnect.hConnection = handle;

        BOOL ret = PInvoke.DeviceIoControl(
            _radioHandle,
            IoctlBthSdpDisconnect,
            &sdpDisconnect,
            (uint)payloadSize,
            null,
            0,
            null,
            null
        );

        if (!ret)
        {
            throw new HostRadioException("Failed to disconnect remote device.", (uint)Marshal.GetLastWin32Error());
        }
    }

    /// <summary>
    ///     Disables the host radio.
    /// </summary>
    /// <exception cref="HostRadioException"></exception>
    public unsafe void DisableRadio()
    {
        byte[] payload = { 0x04, 0x00, 0x00, 0x00 };

        fixed (byte* ptr = payload)
        {
            BOOL ret = PInvoke.DeviceIoControl(
                _radioHandle,
                IoctlSetRadioState,
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
    public unsafe void EnableRadio()
    {
        byte[] payload = { 0x02, 0x00, 0x00, 0x00 };

        fixed (byte* ptr = payload)
        {
            BOOL ret = PInvoke.DeviceIoControl(
                _radioHandle,
                IoctlSetRadioState,
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

    /// <summary>
    ///     Sets the state of a specified service to either enabled or disabled.
    /// </summary>
    /// <param name="address">The remote device address.</param>
    /// <param name="serviceGuid">The service GUID.</param>
    /// <param name="enabled">True to set to enabled, false to disable.</param>
    /// <exception cref="HostRadioException"></exception>
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

    /// <summary>
    ///     Gets the state of a specified service.
    /// </summary>
    /// <param name="address">The remote device address.</param>
    /// <param name="serviceGuid">The service GUID.</param>
    /// <param name="enabled">True if the service is enabled, false otherwise.</param>
    /// <returns>True if the supplied device was found, false otherwise.</returns>
    /// <exception cref="HostRadioException"></exception>
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

    /// <summary>
    ///     Enables advertising a specified service.
    /// </summary>
    /// <param name="serviceGuid">The GUID of the service to expose. This should match the GUID in the server-side INF file.</param>
    /// <param name="serviceName">The service name.</param>
    /// <exception cref="BluetoothServiceException"></exception>
    public void EnableService(Guid serviceGuid, string serviceName)
    {
        BLUETOOTH_LOCAL_SERVICE_INFO_STRUCT svcInfo = new() { szName = serviceName, Enabled = true };

        uint status = PInvoke.BluetoothSetLocalServiceInfo(
            _radioHandle,
            serviceGuid,
            0,
            svcInfo
        );

        if (status != 0)
        {
            throw new BluetoothServiceException("Failed to enable service.", (uint)Marshal.GetLastWin32Error());
        }
    }

    /// <summary>
    ///     Disables advertising a specified service.
    /// </summary>
    /// <param name="serviceGuid">The GUID of the service to expose. This should match the GUID in the server-side INF file.</param>
    /// <param name="serviceName">The service name.</param>
    /// <exception cref="BluetoothServiceException"></exception>
    public void DisableService(Guid serviceGuid, string serviceName)
    {
        BLUETOOTH_LOCAL_SERVICE_INFO_STRUCT svcInfo = new() { szName = serviceName, Enabled = false };

        uint status = PInvoke.BluetoothSetLocalServiceInfo(
            _radioHandle,
            serviceGuid,
            0,
            svcInfo
        );

        if (status != 0)
        {
            throw new BluetoothServiceException("Failed to enable service.", (uint)Marshal.GetLastWin32Error());
        }
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