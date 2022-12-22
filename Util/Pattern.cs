// ReSharper disable RedundantUsingDirective
#if NET6_0_OR_GREATER
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Nefarius.Utilities.Bluetooth.Util;

/// <summary>
///     Utility class to perform pattern matching/finding in byte arrays.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class Pattern
{
    private static bool CheckPattern(string pattern, ReadOnlySpan<byte> input)
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

    /// <summary>
    ///     Attempts to find all occurrences of the provided pattern.
    /// </summary>
    /// <param name="input">The array to search in.</param>
    /// <param name="pattern">The pattern to search for. Use ? or ?? as placeholders for variable content.</param>
    /// <param name="indexes">A list of offsets where the pattern is found in the provided input array.</param>
    public static void FindAll(ReadOnlySpan<byte> input, string pattern, out IEnumerable<int> indexes)
    {
        int elements = pattern.Split(' ').Length;
        List<int> offsets = new();
        int position = 0;

        while (Find(input.Slice(position), pattern, out int offset))
        {
            offsets.Add(position + offset);

            position += offset + elements;
        }

        indexes = offsets;
    }

    /// <summary>
    ///     Attempts to find the first occurrence of the provided pattern.
    /// </summary>
    /// <param name="input">The array to search in.</param>
    /// <param name="pattern">The pattern to search for. Use ? or ?? as placeholders for variable content.</param>
    /// <param name="offset">The zero-based index, if found or -1 otherwise.</param>
    /// <returns>True if the pattern was found, false otherwise.</returns>
    public static bool Find(ReadOnlySpan<byte> input, string pattern, out int offset)
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
                offset = y;
                return true;
            }

            y += pBytes.Length - (pBytes.Length / 2);
        }

        offset = -1;
        return false;
    }
}
#endif