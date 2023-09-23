using System;
using System.Diagnostics.CodeAnalysis;

namespace Nefarius.Utilities.Bluetooth.Exceptions;

/// <summary>
///     Exception potentially thrown by <see cref="HostRadio" />.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class HostRadioException : Exception
{
    internal HostRadioException(string message) : base(message)
    {
    }

    internal HostRadioException(string message, uint errorCode) : this(message)
    {
        NativeErrorCode = errorCode;
    }

    /// <summary>
    ///     Gets the Win32 error code.
    /// </summary>
    public uint NativeErrorCode { get; }
}