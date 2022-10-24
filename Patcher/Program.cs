using Nefarius.Utilities.Bluetooth.SDP;

var bthPortDevices = BthPort.Devices.ToList();

foreach (var device in bthPortDevices)
    if (SdpPatcher.Patch(device.CachedServices.ToArray(), out var patched))
    {
        if (!UtilsConsole.Confirm($"Found device {device}, want me to patch its record?"))
            continue;

        device.CachedServices = patched;

        Console.WriteLine("Patch applied successfully");
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
            if (response != ConsoleKey.Enter) Console.WriteLine();
        } while (response != ConsoleKey.Y && response != ConsoleKey.N);

        return response == ConsoleKey.Y;
    }
}