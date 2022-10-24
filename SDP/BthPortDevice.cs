using System.IO;
using System.Net.NetworkInformation;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace Nefarius.Utilities.Bluetooth.SDP;

public sealed class BthPortDevice
{
    private readonly RegistryKey _cachedServicesKey;

    internal BthPortDevice(RegistryKey parent)
    {
        RemoteAddress = PhysicalAddress.Parse(Path.GetFileName(parent.Name));

        _cachedServicesKey = parent?.OpenSubKey("CachedServices", true);
    }

    [UsedImplicitly]
    public PhysicalAddress RemoteAddress { get; }

    [UsedImplicitly]
    public byte[] CachedServices
    {
        get => (byte[])_cachedServicesKey?.GetValue("00010001");
        set => _cachedServicesKey.SetValue("00010001", value, RegistryValueKind.Binary);
    }

    public override string ToString()
    {
        return RemoteAddress.ToString();
    }
}