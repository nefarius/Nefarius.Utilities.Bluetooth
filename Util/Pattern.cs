// ReSharper disable RedundantUsingDirective
#if NET6_0_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nefarius.Utilities.Bluetooth.Util;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal static class Pattern
{
    private static bool CheckPattern(string pattern, IReadOnlyCollection<byte> input)
    {
        string[] strBytes = pattern.Split(' ');
        int x = 0;
        foreach (byte b in input)
        {
            if (strBytes[x] == "?" || strBytes[x] == "??")
            {
                x++;
            }
            else if (byte.Parse(strBytes[x], NumberStyles.HexNumber) == b)
            {
                x++;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public static int Scan(byte[] input, string pattern)
    {
        string[] pBytes = pattern.Split(' ');

        for (int y = 0; y < input.Length; y++)
        {
            if (input[y] != byte.Parse(pBytes[0], NumberStyles.HexNumber))
            {
                continue;
            }

            byte[] checkArray = new byte[pBytes.Length];

            for (int x = 0; x < pBytes.Length; x++)
            {
                checkArray[x] = input[y + x];
            }

            if (CheckPattern(pattern, checkArray))
            {
                return y;
            }

            y += pBytes.Length - (pBytes.Length / 2);
        }

        return -1;
    }
}
#endif