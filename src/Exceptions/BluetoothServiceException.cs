using System;
using System.Diagnostics.CodeAnalysis;

namespace Nefarius.Utilities.Bluetooth.Exceptions;

/// <summary>
///     Exception potentially thrown by <see cref="HostRadio" />.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class BluetoothServiceException : Exception
{
    internal BluetoothServiceException(string message) : base(message)
    {
    }

    internal BluetoothServiceException(string message, uint errorCode) : this(message)
    {
        NativeErrorCode = errorCode;
    }

    /// <summary>
    ///     Gets the Win32 error code.
    /// </summary>
    public uint NativeErrorCode { get; }
}