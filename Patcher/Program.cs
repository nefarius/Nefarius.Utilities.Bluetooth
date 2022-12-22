using System.Net.NetworkInformation;

using Nefarius.Utilities.Bluetooth;
using Nefarius.Utilities.Bluetooth.SDP;

List<BthPortDevice> bthPortDevices = BthPort.Devices.ToList();

#if TEST
var d = bthPortDevices.First(dev => Equals(dev.RemoteAddress, PhysicalAddress.Parse("748469da44d6")));

var t = SdpPatcher.AlterHidDeviceToVenderDefined(d.CachedServices.ToArray(), out byte[]? p);
#endif

foreach (BthPortDevice? device in bthPortDevices)
{
    if (SdpPatcher.AlterHidDeviceToVenderDefined(device.CachedServices.ToArray(), out byte[]? patched))
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
            radio.RestartRadio();
        }
        else
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
        }
    }
}

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