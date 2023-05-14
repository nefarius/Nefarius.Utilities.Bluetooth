using System;

namespace Nefarius.Utilities.Bluetooth.Exceptions;

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