/*
 * Demo application to test SdpPatcher class.
 *
 * Enumerates all BthPort.Devices, tests if their found records are patchable
 * and asks the user to patch the record from gamepad to vendor device or to
 * undo the patch if it detects a device with a backed up original record.
 */

using Nefarius.Utilities.Bluetooth;
using Nefarius.Utilities.Bluetooth.SDP;
using Nefarius.Utilities.DeviceManagement.PnP;

List<BthPortDevice> bthPortDevices = BthPort.Devices.ToList();

foreach (BthPortDevice? device in bthPortDevices)
{
    if (device.IsCachedServicesPatched)
    {
        if (!UtilsConsole.Confirm($"Found PATCHED device {device}, want me to undo the patch?"))
        {
            continue;
        }

        device.CachedServices = device.OriginalCachedServices;
        device.DeleteOriginalCachedServices();

        Console.WriteLine("Patch reverted successfully");
            
        using var radio = new HostRadio();
        radio.RestartRadio();
        continue;
    }

    if (SdpPatcher.AlterHidDeviceToVenderDefined(device.CachedServices, out byte[]? patched))
    {
        if (!device.IsCachedServicesPatched)
        {
            if (!UtilsConsole.Confirm($"Found device {device}, want me to patch its record?"))
            {
                continue;
            }

            device.CachedServices = patched;

            Console.WriteLine("Patch applied successfully");

            using var radio = new HostRadio();
            //radio.RestartRadio();
            //radio.DisconnectRemoteDevice(device.RemoteAddress);
            radio.SdpConnect(device.RemoteAddress, out var handle);
            //radio.SdpDisconnect(handle);

            //if (Devcon.FindInDeviceClassByHardwareId(
            //        Guid.Parse("{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}"),
            //        @$"BTHENUM\Dev_{device.RemoteAddress}", 
            //        out IEnumerable<string> genInstances)
            //   )
            //{
            //    PnPDevice genericBthDevice = PnPDevice.GetDeviceByInstanceId(genInstances.First());
            //
            //    genericBthDevice.Restart();
            //}

            radio.DisconnectRemoteDevice(device.RemoteAddress);
        }
    }
}

/// <summary>
///     Utility class to create a user prompt at the console.
/// </summary>
internal static class UtilsConsole
{
    public static bool Confirm(string title)
    {
        ConsoleKey response;
        do
        {
            Console.Write($"{title} [y/n] ");
            response = Console.ReadKey(false).Key;
            if (response != ConsoleKey.Enter)
            {
                Console.WriteLine();
            }
        } while (response != ConsoleKey.Y && response != ConsoleKey.N);

        return response == ConsoleKey.Y;
    }
}