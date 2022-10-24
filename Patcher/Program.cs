using Nefarius.Utilities.Bluetooth.SDP;

var bthPortDevices = BthPort.Devices.ToList();

foreach (var device in bthPortDevices)
    if (SdpPatcher.AlterHidDeviceToVenderDefined(device.CachedServices.ToArray(), out var patched))
    {
        Console.WriteLine($"Original record: {string.Join(", ", device.CachedServices.Select(b => $"0x{b:X2}"))}");

        Console.WriteLine($"Patched record: {string.Join(", ", patched.Select(b => $"0x{b:X2}"))}");

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