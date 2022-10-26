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
        set
        {
            // make backup copy
            _cachedServicesKey.SetValue("Nefarius-00010001-Backup", CachedServices, RegistryValueKind.Binary);
            // update original value
            _cachedServicesKey.SetValue("00010001", value, RegistryValueKind.Binary);
        }
    }

    [UsedImplicitly]
    public bool IsCachedServicesPatched
    {
        get
        {
            byte[] content = (byte[])_cachedServicesKey?.GetValue("Nefarius-00010001-Backup");

            return content is not null;
        }
    }

    [UsedImplicitly]
    public byte[] OriginalCachedServices
    {
        get
        {
            if (!IsCachedServicesPatched)
            {
                return CachedServices;
            }

            return (byte[])_cachedServicesKey?.GetValue("Nefarius-00010001-Backup");
        }
    }

    public void DeleteOriginalCachedServices()
    {
        if (!IsCachedServicesPatched)
        {
            return;
        }

        _cachedServicesKey?.DeleteValue("Nefarius-00010001-Backup");
    }

    public override string ToString()
    {
        return RemoteAddress.ToString();
    }
}