using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Win32;

namespace Nefarius.Utilities.Bluetooth.SDP;

/// <summary>
///     Wrapper around BTHPORT.SYS registry structure.
/// </summary>
/// <remarks>Use of this class requires elevated privileges.</remarks>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class BthPort
{
    private static readonly List<string> ValidValueNames = ["00010000", "00010001"];

    /// <summary>
    ///     Device interface GUID.
    /// </summary>
    public static Guid DeviceInterface => Guid.Parse("{0850302a-b344-4fda-9be9-90576b8d46f0}");

    /// <summary>
    ///     Gets a collection of paired HID devices.
    /// </summary>
    public static IEnumerable<BthPortDevice> Devices
    {
        get
        {
            using RegistryKey devicesKey =
                Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices");

            if (devicesKey is not null)
            {
                string[] devicesKeyNames = devicesKey.GetSubKeyNames();

                foreach (string keyName in devicesKeyNames)
                {
                    RegistryKey device = devicesKey.OpenSubKey(keyName);

                    /*
                     * "DynamicCachedServices" exists as well, but it appears that the
                     * content in "CachedServices" takes priority when values are modified.
                     */
                    RegistryKey cachedServices = device?.OpenSubKey("CachedServices");

                    if (cachedServices is null)
                    {
                        continue;
                    }

                    // Check if content is of interest
                    string valueName = FindValueNameContainingRecords(cachedServices);

                    if (valueName is null)
                    {
                        continue;
                    }

                    /*
                     * The SDP record(s) containing the attributes of interest are stored in this value.
                     * The format is exactly the same as it travels through the air (Wireshark observable).
                     * During operation, the content is cached in kernel memory, so if it's changed during
                     * runtime, the driver needs to unload and load again
                     * (radio restart, service restart etc.).
                     */
                    byte[] value = (byte[])cachedServices.GetValue(valueName);

                    if (value is null)
                    {
                        continue;
                    }

                    yield return new BthPortDevice(device);
                }
            }
        }
    }

    internal static string FindValueNameContainingRecords(RegistryKey cachedServices)
    {
        string[] values = cachedServices.GetValueNames();

        if (values.Length == 0)
        {
            return null;
        }

        if (!values.Intersect(ValidValueNames).Any())
        {
            return null;
        }

        string valueName =
            (
                from name in values
                let content = (byte[])cachedServices.GetValue(name)
                where content is not null && content.Length > 0 && content[0] == 0x36
                select name
            )
            .FirstOrDefault();

        return valueName;
    }
}