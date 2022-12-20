using System;
using System.Net.NetworkInformation;

namespace Nefarius.Utilities.Bluetooth;

/// <summary>
///     Describes a remote wireless device.
/// </summary>
public sealed class RemoteDevice : IEquatable<RemoteDevice>
{
    /// <summary>
    ///     Gets the reported remote device name.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    ///     Gets the unique remote device address.
    /// </summary>
    public PhysicalAddress Address { get; internal set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name} ({Address})";
    }

    /// <inheritdoc />
    public bool Equals(RemoteDevice other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Equals(Address, other.Address);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || (obj is RemoteDevice other && Equals(other));
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Address != null ? Address.GetHashCode() : 0;
    }

#pragma warning disable CS1591
    public static bool operator ==(RemoteDevice left, RemoteDevice right)
#pragma warning restore CS1591
    {
        return Equals(left, right);
    }

#pragma warning disable CS1591
    public static bool operator !=(RemoteDevice left, RemoteDevice right)
#pragma warning restore CS1591
    {
        return !Equals(left, right);
    }
}