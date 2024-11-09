using System;
using System.Diagnostics.CodeAnalysis;

using Windows.Win32.Foundation;

using Nefarius.Utilities.DeviceManagement.Exceptions;

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

    /// <inheritdoc />
    public override string Message => NativeErrorCode == (uint)WIN32_ERROR.ERROR_SUCCESS
        ? base.Message
        : $"{base.Message} (code: {NativeErrorCode}, message: {Win32Exception.GetMessageFor((int)NativeErrorCode)})";

    /// <summary>
    ///     Gets the Win32 error code.
    /// </summary>
    public uint NativeErrorCode { get; } = (uint)WIN32_ERROR.ERROR_SUCCESS;
}