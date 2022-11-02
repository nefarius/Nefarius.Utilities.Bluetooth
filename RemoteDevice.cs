using System;
using System.Net.NetworkInformation;

namespace Nefarius.Utilities.Bluetooth;

public sealed class RemoteDevice : IEquatable<RemoteDevice>
{
    public string Name { get; internal set; }

    public PhysicalAddress Address { get; internal set; }

    public override string ToString()
    {
        return $"{Name} ({Address})";
    }

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

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || (obj is RemoteDevice other && Equals(other));
    }

    public override int GetHashCode()
    {
        return Address != null ? Address.GetHashCode() : 0;
    }

    public static bool operator ==(RemoteDevice left, RemoteDevice right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RemoteDevice left, RemoteDevice right)
    {
        return !Equals(left, right);
    }
}