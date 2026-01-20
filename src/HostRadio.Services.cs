using System;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Devices.Bluetooth;
using Windows.Win32.Foundation;
using Nefarius.Utilities.Bluetooth.Exceptions;

namespace Nefarius.Utilities.Bluetooth;

public sealed partial class HostRadio
{
    /// <summary>
    ///     HID Service Class GUID.
    /// </summary>
    public static Guid HumanInterfaceDeviceServiceClassUuid =>
        Guid.Parse("{0x1124,0x0000,0x1000,{0x80,0x00,0x00,0x80,0x5F,0x9B,0x34,0xFB}}");

    /// <summary>
    ///     Enables advertising a specified service.
    /// </summary>
    /// <remarks>This method requires administrative privileges.</remarks>
    /// <param name="serviceGuid">The GUID of the service to expose. This should match the GUID in the server-side INF file.</param>
    /// <param name="serviceName">The service name.</param>
    /// <exception cref="BluetoothServiceException"></exception>
    /// <exception cref="AdjustProcessPrivilegesException"></exception>
    public void EnableService(Guid serviceGuid, string serviceName)
    {
        AdjustProcessPrivileges();

        BLUETOOTH_LOCAL_SERVICE_INFO svcInfo = new() { Enabled = true, szName = serviceName };

        _ = PInvoke.BluetoothSetLocalServiceInfo(
            _radioHandle,
            serviceGuid,
            0,
            svcInfo
        );

        var error = (WIN32_ERROR)Marshal.GetLastWin32Error();

        if (error != WIN32_ERROR.ERROR_SUCCESS)
            throw new BluetoothServiceException("Failed to enable service.", (uint)error);
    }

    /// <summary>
    ///     Disables advertising a specified service.
    /// </summary>
    /// <remarks>This method requires administrative privileges.</remarks>
    /// <param name="serviceGuid">The GUID of the service to expose. This should match the GUID in the server-side INF file.</param>
    /// <param name="serviceName">The service name.</param>
    /// <exception cref="BluetoothServiceException"></exception>
    /// <exception cref="AdjustProcessPrivilegesException"></exception>
    public void DisableService(Guid serviceGuid, string serviceName)
    {
        AdjustProcessPrivileges();

        BLUETOOTH_LOCAL_SERVICE_INFO svcInfo = new() { Enabled = false, szName = serviceName };

        _ = PInvoke.BluetoothSetLocalServiceInfo(
            _radioHandle,
            serviceGuid,
            0,
            svcInfo
        );

        var error = (WIN32_ERROR)Marshal.GetLastWin32Error();

        if (error != WIN32_ERROR.ERROR_SUCCESS)
            throw new BluetoothServiceException("Failed to enable service.", (uint)error);
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
        var found = FindDeviceByAddress(address, out var deviceInfo);

        if (!found) return;

        var flags = enabled ? PInvoke.BLUETOOTH_SERVICE_ENABLE : PInvoke.BLUETOOTH_SERVICE_DISABLE;

        var error = PInvoke.BluetoothSetServiceState(_radioHandle, deviceInfo, serviceGuid, flags);

        if (error != (uint)WIN32_ERROR.ERROR_SUCCESS)
            throw new HostRadioException("Failed to set service state.", error);
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

        var found = FindDeviceByAddress(address, out var deviceInfo);

        if (!found) return false;

        uint numServices = 0;
        var error = PInvoke.BluetoothEnumerateInstalledServices(_radioHandle, deviceInfo, ref numServices, null);

        if (error == (uint)WIN32_ERROR.ERROR_SUCCESS && numServices == 0)
        {
            enabled = false;
            return true;
        }

        if (error != (uint)WIN32_ERROR.ERROR_SUCCESS && error != (uint)WIN32_ERROR.ERROR_MORE_DATA)
            throw new HostRadioException("Failed to enumerate services.", error);

        Span<Guid> services = stackalloc Guid[(int)numServices];
        error = PInvoke.BluetoothEnumerateInstalledServices(_radioHandle, deviceInfo, ref numServices, services);

        if (error != (uint)WIN32_ERROR.ERROR_SUCCESS)
            throw new HostRadioException("Failed to enumerate services.", error);

        for (var i = 0; i < numServices; i++)
        {
            var uuid = services[i];

            if (uuid != serviceGuid) continue;

            enabled = true;
            return true;
        }

        return false;
    }
}