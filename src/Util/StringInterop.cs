using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Nefarius.Utilities.Bluetooth.Util;

using winmdroot = Windows.Win32;

internal static class StringInterop
{
    public static void AssignFromString(this winmdroot.__char_256 array, string value)
    {
        if (value.Length > array.Length + Marshal.SizeOf<char>())
        {
            throw new ArgumentOutOfRangeException(nameof(value), "The provided string value is too large to fit.");
        }

        byte[] bytes = Encoding.UTF8.GetBytes(value);

        unsafe
        {
            for (int i = 0; i < value.Length; i++)
            {
                array.Value[i] = (char)bytes[i];
            }
        }
    }
}