using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Devices.Bluetooth;

using Nefarius.Utilities.Bluetooth.Util;

namespace Nefarius.Utilities.Bluetooth;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed partial class HostRadio
{
    /// <summary>
    ///     Gets all remote devices.
    /// </summary>
    public IEnumerable<RemoteDevice> AllDevices =>
        AuthenticatedDevices
            .Concat(ConnectedDevices)
            .Concat(RememberedDevices)
            .Concat(UnknownDevices)
            .Distinct();

    /// <summary>
    ///     Gets all authenticated devices.
    /// </summary>
    public IEnumerable<RemoteDevice> AuthenticatedDevices
    {
        get
        {
            BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams = new()
            {
                dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_SEARCH_PARAMS>(),
                fReturnAuthenticated = true,
                hRadio = _radioHandle.ToHandle()
            };

            BLUETOOTH_DEVICE_INFO deviceInfo =
                new() { dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_INFO>() };

            using BluetoothFindDeviceCloseSafeHandle hFind =
                PInvoke.BluetoothFindFirstDevice(searchParams, ref deviceInfo);

            if (!hFind.IsInvalid)
            {
                do
                {
                    yield return new RemoteDevice(deviceInfo.szName.ToString(), new PhysicalAddress(deviceInfo
#if NETSTANDARD
                        .Address.Anonymous.rgBytes.ToArray().Reverse()
#else
                        .Address.Anonymous.rgBytes.AsSpan().ToArray().Reverse()
#endif
                        .ToArray()));
                } while (PInvoke.BluetoothFindNextDevice(hFind, ref deviceInfo));
            }
        }
    }

    /// <summary>
    ///     Gets all connected devices.
    /// </summary>
    public IEnumerable<RemoteDevice> ConnectedDevices
    {
        get
        {
            BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams = new()
            {
                dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_SEARCH_PARAMS>(),
                fReturnConnected = true,
                hRadio = _radioHandle.ToHandle()
            };

            BLUETOOTH_DEVICE_INFO deviceInfo =
                new() { dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_INFO>() };

            using BluetoothFindDeviceCloseSafeHandle hFind =
                PInvoke.BluetoothFindFirstDevice(searchParams, ref deviceInfo);

            if (!hFind.IsInvalid)
            {
                do
                {
                    yield return new RemoteDevice(deviceInfo.szName.ToString(), new PhysicalAddress(deviceInfo
#if NETSTANDARD
                        .Address.Anonymous.rgBytes.ToArray().Reverse()
#else
                        .Address.Anonymous.rgBytes.AsSpan().ToArray().Reverse()
#endif
                        .ToArray()));
                } while (PInvoke.BluetoothFindNextDevice(hFind, ref deviceInfo));
            }
        }
    }

    /// <summary>
    ///     Gets all remembered devices.
    /// </summary>
    public IEnumerable<RemoteDevice> RememberedDevices
    {
        get
        {
            BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams = new()
            {
                dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_SEARCH_PARAMS>(),
                fReturnRemembered = true,
                hRadio = _radioHandle.ToHandle()
            };

            BLUETOOTH_DEVICE_INFO deviceInfo =
                new() { dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_INFO>() };

            using BluetoothFindDeviceCloseSafeHandle hFind =
                PInvoke.BluetoothFindFirstDevice(searchParams, ref deviceInfo);

            if (!hFind.IsInvalid)
            {
                do
                {
                    yield return new RemoteDevice
                    (deviceInfo.szName.ToString(),
                        new PhysicalAddress(deviceInfo
#if NETSTANDARD
                            .Address.Anonymous.rgBytes.ToArray().Reverse()
#else
                            .Address.Anonymous.rgBytes.AsSpan().ToArray().Reverse()
#endif
                            .ToArray()
                        ));
                } while (PInvoke.BluetoothFindNextDevice(hFind, ref deviceInfo));
            }
        }
    }

    /// <summary>
    ///     Gets all unknown devices.
    /// </summary>
    public IEnumerable<RemoteDevice> UnknownDevices
    {
        get
        {
            BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams = new()
            {
                dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_SEARCH_PARAMS>(),
                fReturnUnknown = true,
                hRadio = _radioHandle.ToHandle()
            };

            BLUETOOTH_DEVICE_INFO deviceInfo =
                new() { dwSize = (uint)Marshal.SizeOf<BLUETOOTH_DEVICE_INFO>() };

            using BluetoothFindDeviceCloseSafeHandle hFind =
                PInvoke.BluetoothFindFirstDevice(searchParams, ref deviceInfo);

            if (!hFind.IsInvalid)
            {
                do
                {
                    yield return new RemoteDevice
                    (deviceInfo.szName.ToString(),
                        new PhysicalAddress(deviceInfo
#if NETSTANDARD
                            .Address.Anonymous.rgBytes.ToArray().Reverse()
#else
                            .Address.Anonymous.rgBytes.AsSpan().ToArray().Reverse()
#endif
                            .ToArray()));
                } while (PInvoke.BluetoothFindNextDevice(hFind, ref deviceInfo));
            }
        }
    }
}