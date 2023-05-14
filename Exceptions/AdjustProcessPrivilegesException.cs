using System;
using System.Diagnostics.CodeAnalysis;

namespace Nefarius.Utilities.Bluetooth.Exceptions;

/// <summary>
///     Thrown when privilege adjustments failed.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class AdjustProcessPrivilegesException : Exception
{
    internal AdjustProcessPrivilegesException(string message) : base(message)
    {
    }

    internal AdjustProcessPrivilegesException(string message, uint errorCode) : this(message)
    {
        NativeErrorCode = errorCode;
    }

    /// <summary>
    ///     Gets the Win32 error code.
    /// </summary>
    public uint NativeErrorCode { get; }
}