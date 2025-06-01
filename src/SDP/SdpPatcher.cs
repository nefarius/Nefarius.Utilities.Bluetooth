// ReSharper disable RedundantUsingDirective

#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;

using Nefarius.Utilities.Bluetooth.Util;

namespace Nefarius.Utilities.Bluetooth.SDP;

/// <summary>
///     Service Discovery Record Patching Utility.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static class SdpPatcher
{
    /// <summary>
    ///     Attempts to find an SDP_ATTRIB_HID_DESCRIPTOR_LIST attribute and patches the HID Report Descriptor to a Vendor
    ///     Defined device on success.
    /// </summary>
    /// <param name="input">The original record array.</param>
    /// <param name="output">The patched record array.</param>
    /// <returns>True if detection and patching were successful, false otherwise.</returns>
    public static bool AlterHidDeviceToVendorDefined(byte[] input, out byte[] output)
    {
        try
        {
            // find beginning of SDP_ATTRIB_HID_DESCRIPTOR_LIST
            byte[] descriptorListStartPattern = [0x09, 0x02, 0x06, 0x36];
            int descriptorListStartIndex = new BoyerMoore(descriptorListStartPattern).Search(input).First();

            // skip to start of next element
            int descriptorValueStartOffset = descriptorListStartPattern.Length + descriptorListStartIndex;

            // find beginning of actual HID Report Descriptor buffer
            byte[] descriptorValueStartPattern = [0x08, 0x22, 0x26];
            int descriptorValueStartIndex = new BoyerMoore(descriptorValueStartPattern)
                .Search(input.Skip(descriptorValueStartOffset).ToArray()).First();

            // find report size value offset
            int dataElementSizeIndex = descriptorListStartPattern.Length +
                                       descriptorListStartIndex +
                                       descriptorValueStartPattern.Length +
                                       descriptorValueStartIndex;

            // get buffer size in bytes
            ushort dataElementSize = BitConverter.ToUInt16(input
                .Skip(dataElementSizeIndex)
                .Take(2).Reverse().ToArray());

            // start offset of report descriptor buffer
            int dataBufferValueIndex = dataElementSizeIndex + 2;
            // extract HID Report Descriptor
            byte[] hidReportDescriptor = input.Skip(dataBufferValueIndex).Take(dataElementSize).ToArray();

            // find all size fields before the descriptor element
            List<int> sizeFieldsOffsets = new BoyerMoore([0x36])
                .Search(input.Take(dataElementSizeIndex).ToArray()).ToList();

            // copy descriptor
            List<byte> patchedHidReportDescriptor = hidReportDescriptor.ToList();

            // report parser
            HidReportDescriptorParser parser = new();

            byte usageIndex = 0x01;

            // look for Usage Pages
            parser.GlobalItemParsed += item =>
            {
                // original pages have 1 byte size, patched are 2 bytes
                if (!item.IsUsagePage || item.ItemSize != 1)
                {
                    return;
                }

                // remove existing
                patchedHidReportDescriptor.RemoveRange(item.Index, item.ItemSize + 1);
                // splice in replacement
                patchedHidReportDescriptor.InsertRange(item.Index, [0x06, usageIndex++, 0xFF]);

                // recursively parse until no more original pages are left
                parser.Parse(patchedHidReportDescriptor.ToArray());

                // abort previous parser run
                item.StopParsing = true;
            };

            // kick off initial parser run
            parser.Parse(patchedHidReportDescriptor.ToArray());

            List<byte> patchedRecord = [];

            int lastOffset = 0;

            // iterate through size segments to recalculate them
            foreach (int offset in sizeFieldsOffsets)
            {
                // calculate size of the segment to take from original record
                int previousSegmentLength = Math.Clamp(offset - lastOffset, 0, input.Length);
                // get segment to splice in
                IEnumerable<byte> previousSegment = input.Skip(lastOffset).Take(previousSegmentLength);

                patchedRecord.AddRange(previousSegment);

                // extract size content
                ushort sizeFieldValue = BitConverter.ToUInt16(input
                    .Skip(offset + 1 /* skip field type itself */)
                    .Take(2).Reverse().ToArray());

                // adjust size depending on report size difference
                int newSize = sizeFieldValue + (patchedHidReportDescriptor.Count - hidReportDescriptor.Length);

                // add new size segment
                patchedRecord.Add(0x36);
                patchedRecord.AddRange(BitConverter.GetBytes((ushort)newSize).Reverse());

                lastOffset = offset + 3; // need to skip the segment we created
            }

            // add start pattern
            patchedRecord.AddRange(descriptorValueStartPattern);

            // add size of patched descriptor
            patchedRecord.AddRange(BitConverter.GetBytes((ushort)patchedHidReportDescriptor.Count).Reverse());

            // add patched descriptor
            patchedRecord.AddRange(patchedHidReportDescriptor);

            // add remaining record segments
            patchedRecord.AddRange(input.Skip(dataBufferValueIndex + hidReportDescriptor.Length));

            output = patchedRecord.ToArray();

            return true;
        }
        catch (InvalidOperationException)
        {
            return TryFallbackMethod(input, out output);
        }
    }

    /// <summary>
    ///     Method for devices with records like Nintendo Switch Pro Controller and alike.
    /// </summary>
    private static bool TryFallbackMethod(byte[] input, out byte[] output)
    {
        try
        {
            // Find Service Attribute: (HID) Descriptor List (0x206), value = Report
            byte[] descriptorListStartPattern = [0x09, 0x02, 0x06];
            int descriptorListStartIndex = new BoyerMoore(descriptorListStartPattern).Search(input).First();
            byte[] descriptorList = input.Skip(descriptorListStartIndex).ToArray();

            // pattern to find uint8_t size elements
            const string pattern = "35 ??";
            // find all size element offsets
            Pattern.FindAll(descriptorList, pattern, out IEnumerable<int> sizeElementsIndexes);

            // map size element offset to current size value
            Dictionary<int, byte> sizeElements = sizeElementsIndexes
                .Take(2) // not interested in fields below descriptor list
                .ToDictionary(sizeElementsIndex => descriptorListStartIndex + sizeElementsIndex + 1,
                    sizeElementsIndex => descriptorList[sizeElementsIndex + 1]);

            // Find HID Report Descriptor
            byte[] descriptorStartPattern = [0x05, 0x01, 0x09, 0x05];
            int descriptorStartIndex = new BoyerMoore(descriptorStartPattern).Search(input).First();

            // get descriptor length
            int descriptorSizeIndex = descriptorStartIndex - 1;
            byte descriptorSize = input[descriptorSizeIndex];
            int descriptorEnd = descriptorStartIndex + descriptorSize;

            // get original descriptor
            List<byte> hidReportDescriptor = input.Skip(descriptorStartIndex).Take(descriptorSize).ToList();

            // copy descriptor
            List<byte> patchedHidReportDescriptor = hidReportDescriptor.ToList();

            // report parser
            HidReportDescriptorParser parser = new();

            byte usageIndex = 0x01;

            // look for Usage Pages
            parser.GlobalItemParsed += item =>
            {
                // original pages have 1 byte size, patched are 2 bytes
                if (!item.IsUsagePage || item.ItemSize != 1)
                {
                    return;
                }

                // remove existing
                patchedHidReportDescriptor.RemoveRange(item.Index, item.ItemSize + 1);
                // splice in replacement
                patchedHidReportDescriptor.InsertRange(item.Index, [0x06, usageIndex++, 0xFF]);

                // recursively parse until no more original pages are left
                parser.Parse(patchedHidReportDescriptor.ToArray());

                // abort previous parser run
                item.StopParsing = true;
            };

            // kick off initial parser run
            parser.Parse(patchedHidReportDescriptor.ToArray());

            // new descriptor will be expected to be bigger, vendor usages require more bytes
            int patchedDescriptorSize = patchedHidReportDescriptor.Count;

            // need to add the growth difference to list size elements
            int sizeDifference = patchedDescriptorSize - descriptorSize;

            // prepare new records
            List<byte> patchedRecord = [];

            // take everything before original descriptor blob
            patchedRecord.AddRange(input.Take(descriptorStartIndex));

            // patch element sizes
            foreach (KeyValuePair<int, byte> sizeElement in sizeElements)
            {
                patchedRecord[sizeElement.Key] = (byte)(sizeElement.Value + sizeDifference);
            }

            // patch new descriptor length
            patchedRecord[descriptorSizeIndex] = (byte)patchedDescriptorSize;

            // add patched descriptor blob
            patchedRecord.AddRange(patchedHidReportDescriptor);

            // add remaining, unaltered blobs
            patchedRecord.AddRange(input.Skip(descriptorEnd));

            // done!
            output = patchedRecord.ToArray();
            return true;

            //var hex = string.Join(" ", descriptor.Select(b => $"{b:X2} "));
        }
        catch (InvalidOperationException)
        {
            output = null;
            return false;
        }
    }
}
#endif
