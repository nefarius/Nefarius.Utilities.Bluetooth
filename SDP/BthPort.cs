using System.Collections.Generic;
using Microsoft.Win32;

namespace Nefarius.Utilities.Bluetooth.SDP;

public static class BthPort
{
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