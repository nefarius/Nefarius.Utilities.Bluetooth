using System;

using Windows.Win32.Foundation;

using Microsoft.Win32.SafeHandles;

namespace Nefarius.Utilities.Bluetooth.Util;

internal static class SafeFileHandleExtensions
{
    public static unsafe HANDLE ToHandle(this SafeFileHandle handle)
    {
        return new HANDLE(handle.DangerousGetHandle().ToPointer());
    }
}