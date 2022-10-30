using Nefarius.Utilities.Bluetooth;
using Nefarius.Utilities.Bluetooth.SDP;

using HostRadio radio = new HostRadio();

radio.EnableRadio();

List<BthPortDevice> bthPortDevices = BthPort.Devices.ToList();

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