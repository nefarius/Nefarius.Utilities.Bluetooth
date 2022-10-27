using System.Collections.Generic;
using Microsoft.Win32;

namespace Nefarius.Utilities.Bluetooth.SDP;

/// <summary>
///     Wrapper around BTHPORT.SYS
/// </summary>
public static class BthPort
{
    /// <summary>
    ///     Gets a collection of paired HID devices.
    /// </summary>
    public static IEnumerable<BthPortDevice> Devices
    {
        get
        {
            using var devicesKey =
                Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\BTHPORT\\Parameters\\Devices");

            if (devicesKey is not null)
            {
                var devicesKeyNames = devicesKey.GetSubKeyNames();

                foreach (var keyName in devicesKeyNames)
                {
                    var device = devicesKey.OpenSubKey(keyName);

                    var cachedServices = device?.OpenSubKey("CachedServices");

                    var value = (byte[])cachedServices?.GetValue("00010001");

                    if (value is null)
                        continue;

                    yield return new BthPortDevice(device);
                }
            }
        }
    }
}