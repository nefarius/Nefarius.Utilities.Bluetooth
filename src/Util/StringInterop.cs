using System;
using System.Text;

using Windows.Win32.Foundation;

namespace Nefarius.Utilities.Bluetooth.Util;

using winmdroot = Windows.Win32;

internal static class StringInterop
{
    public static unsafe void AssignFromString(this winmdroot.__char_256 array, string value)
    {
        Int32 iNewDataLen = winmdroot.PInvoke.WideCharToMultiByte(
            Convert.ToUInt32(Encoding.UTF8.CodePage),
            0, value,
            value.Length,
            null,
            0,
            string.Empty,
            null
        );

        if (iNewDataLen <= 1)
        {
            return;
        }

        byte* buffer = stackalloc byte[iNewDataLen];
        winmdroot.PInvoke.WideCharToMultiByte(
            Convert.ToUInt32(Encoding.UTF8.CodePage),
            0,
            value, -1,
            new PSTR(buffer), iNewDataLen, string.Empty, null);

        Buffer.MemoryCopy(buffer, &array, array.Length, iNewDataLen);
    }
}