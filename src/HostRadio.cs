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
using Nefarius.Utilities.DeviceManagement.Exceptions;
using Nefarius.Utilities.DeviceManagement.Extensions;
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
    ///     Creates a new instance of <see cref="HostRadio" />.
    /// </summary>
    /// <param name="autoEnable">
    ///     True to automatically enable the radio if currently disabled, false will throw an exception.
    ///     You can also use <see cref="IsAvailable" /> to avoid this exception.
    /// </param>
    /// <exception cref="HostRadioException">Radio handle access has failed.</exception>
    public HostRadio(bool autoEnable = true)
    {
        BLUETOOTH_FIND_RADIO_PARAMS radioParams;
        radioParams.dwSize = (uint)Marshal.SizeOf<BLUETOOTH_FIND_RADIO_PARAMS>();

        using BluetoothFindRadioCloseSafeHandle hFind = PInvoke.BluetoothFindFirstRadio(radioParams, out _radioHandle);

        if (!hFind.IsInvalid)
        {
            return;
        }

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
            (uint)(FILE_ACCESS_RIGHTS.FILE_GENERIC_READ | FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE),
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

    /// <summary>
    ///     Gets whether a host radio is available (and can be enabled).
    /// </summary>
    public static bool IsAvailable => Devcon.FindByInterfaceGuid(DeviceInterface, out _, out _);

    /// <summary>
    ///     Gets whether a host radio is enabled.
    /// </summary>
    public static bool IsEnabled
    {
        get
        {
            BLUETOOTH_FIND_RADIO_PARAMS radioParams;
            radioParams.dwSize = (uint)Marshal.SizeOf<BLUETOOTH_FIND_RADIO_PARAMS>();

            using BluetoothFindRadioCloseSafeHandle hFind = PInvoke.BluetoothFindFirstRadio(radioParams, out _);

            return !hFind.IsInvalid;
        }
    }

    /// <summary>
    ///     Gets whether a host radio is available and enabled.
    /// </summary>
    public static bool IsOperable => IsAvailable && IsEnabled;

    /// <summary>
    ///     Device Interface GUID.
    /// </summary>
    public static Guid DeviceInterface => Guid.Parse("{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}");

    /// <inheritdoc />
    public void Dispose()
    {
        _radioHandle?.Dispose();
    }

    /// <summary>
    ///     Restarts the radio bus device. For USB devices this is achieved by requesting a port cycle from the attached hub.
    /// </summary>
    /// <remarks>Requires administrative privileges. Currently only USB devices are supported.</remarks>
    /// <exception cref="HostRadioException"></exception>
    /// <exception cref="UsbPnPDeviceConversionException"></exception>
    public static void RestartRadioDevice()
    {
        if (!Devcon.FindByInterfaceGuid(DeviceInterface, out PnPDevice device))
        {
            throw new HostRadioException("Bluetooth host radio not found.", (uint)Marshal.GetLastWin32Error());
        }

        UsbPnPDevice usbDevice = device.ToUsbPnPDevice();
        usbDevice.CyclePort();
    }

    /// <summary>
    ///     Instruct host radio to disconnect a given remote device.
    /// </summary>
    /// <param name="address">A parseable MAC address string.</param>
    /// <remarks>
    ///     See
    ///     https://learn.microsoft.com/en-us/dotnet/api/system.net.networkinformation.physicaladdress.parse for
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
            throw new HostRadioException("Failed to connect remote device.", (uint)Marshal.GetLastWin32Error());
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
    /// <remarks>
    ///     This causes the FDO of the radio bus driver to report all its child devices as absent (basically tearing down
    ///     the entire stack, excluding itself). This has the same effect as a user switching off Bluetooth from the Windows
    ///     UI.
    /// </remarks>
    /// <exception cref="HostRadioException"></exception>
    public unsafe void DisableRadio()
    {
        byte[] payload = [0x04, 0x00, 0x00, 0x00];

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
    /// <remarks>
    ///     This causes the FDO of the radio bus driver to enumerate all its child devices and mark them as present. This has
    ///     the same effect as a user switching on Bluetooth from the Windows UI.
    /// </remarks>
    /// <exception cref="HostRadioException"></exception>
    public unsafe void EnableRadio()
    {
        byte[] payload = [0x02, 0x00, 0x00, 0x00];

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
}