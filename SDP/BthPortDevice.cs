﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.NetworkInformation;

using Microsoft.Win32;

namespace Nefarius.Utilities.Bluetooth.SDP;

/// <summary>
///     Represents a device entry of BTHPORT.SYS
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed class BthPortDevice
{
    private readonly RegistryKey _cachedServicesKey;

    internal BthPortDevice(RegistryKey parent)
    {
        RemoteAddress = PhysicalAddress.Parse(Path.GetFileName(parent.Name));

        _cachedServicesKey = parent?.OpenSubKey("CachedServices", true);
    }

    /// <summary>
    ///     The remote device MAC address.
    /// </summary>
    public PhysicalAddress RemoteAddress { get; }

    /// <summary>
    ///     Gets the cached service records blob.
    /// </summary>
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

    /// <summary>
    ///     True if <see cref="CachedServices" /> has been altered, false otherwise.
    /// </summary>
    public bool IsCachedServicesPatched
    {
        get
        {
            byte[] content = (byte[])_cachedServicesKey?.GetValue("Nefarius-00010001-Backup");

            return content is not null;
        }
    }

    /// <summary>
    ///     Gets the unmodified cached service records blob.
    /// </summary>
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

    /// <summary>
    ///     Delete the backup record blob.
    /// </summary>
    public void DeleteOriginalCachedServices()
    {
        if (!IsCachedServicesPatched)
        {
            return;
        }

        _cachedServicesKey?.DeleteValue("Nefarius-00010001-Backup");
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return RemoteAddress.ToString();
    }
}