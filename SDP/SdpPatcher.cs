using System;
using System.Collections.Generic;
using System.Linq;
using Nefarius.Utilities.Bluetooth.Util;

namespace Nefarius.Utilities.Bluetooth.SDP;

/// <summary>
///     Service Discovery Record Patching Utility.
/// </summary>
public static class SdpPatcher
{
    /// <summary>
    ///     Attempts to find a SDP_ATTRIB_HID_DESCRIPTOR_LIST attribute and patches the HID Report Descriptor to a Vendor
    ///     Defined device on success.
    /// </summary>
    /// <param name="input">The original record array.</param>
    /// <param name="output">The patched record array.</param>
    /// <returns>True if detection and patching was successful, false otherwise.</returns>
    public static bool AlterHidDeviceToVenderDefined(byte[] input, out byte[] output)
    {
        try
        {
            // find beginning of SDP_ATTRIB_HID_DESCRIPTOR_LIST
            var descriptorListStartPattern = new byte[] { 0x09, 0x02, 0x06, 0x36 };
            var descriptorListStartIndex = new BoyerMoore(descriptorListStartPattern).Search(input).First();

            // skip to start of next element
            var descriptorValueStartOffset = descriptorListStartPattern.Length + descriptorListStartIndex;

            // find beginning of actual HID Report Descriptor buffer
            var descriptorValueStartPattern = new byte[] { 0x08, 0x22, 0x26 };
            var descriptorValueStartIndex = new BoyerMoore(descriptorValueStartPattern)
                .Search(input.Skip(descriptorValueStartOffset).ToArray()).First();

            // find report size value offset
            var dataElementSizeIndex = descriptorListStartPattern.Length +
                                       descriptorListStartIndex +
                                       descriptorValueStartPattern.Length +
                                       descriptorValueStartIndex;

            // get buffer size in bytes
            var dataElementSize = BitConverter.ToUInt16(input
                .Skip(dataElementSizeIndex)
                .Take(2).Reverse().ToArray());

            // start offset of report descriptor buffer
            var dataBufferValueIndex = dataElementSizeIndex + 2;
            // extract HID Report Descriptor
            var hidReportDescriptor = input.Skip(dataBufferValueIndex).Take(dataElementSize).ToArray();

            // vendor HID device usage header
            var usagePageVendorDevice = new byte[]
            {
                0x06, 0x01, 0xFF, // Usage Page (Vendor Defined 0xFF01)
                0x09, 0x01 // Usage (0x01)
            };

            // find all size fields before the descriptor element
            var sizeFieldsOffsets = new BoyerMoore(new byte[] { 0x36 })
                .Search(input.Take(dataElementSizeIndex).ToArray()).ToList();

            var patchedRecord = new List<byte>();

            var lastOffset = 0;

            // iterate through size segments to recalculate them
            foreach (var offset in sizeFieldsOffsets)
            {
                // calculate size of the segment to take from original record
                var previousSegmentLength = Math.Clamp(offset - lastOffset, 0, input.Length);
                // get segment to splice in
                var previousSegment = input.Skip(lastOffset).Take(previousSegmentLength);

                patchedRecord.AddRange(previousSegment);

                // extract size content
                var sizeFieldValue = BitConverter.ToUInt16(input
                    .Skip(offset + 1 /* skip field type itself */)
                    .Take(2).Reverse().ToArray());

                // we only need one extra byte
                var newSize = sizeFieldValue + 1;

                // add new size segment
                patchedRecord.Add(0x36);
                patchedRecord.AddRange(BitConverter.GetBytes((ushort)newSize).Reverse());

                lastOffset = offset + 3; // need to skip the segment we created
            }

            // add start pattern
            patchedRecord.AddRange(descriptorValueStartPattern);

            // build patched descriptor
            var patchedHidReportDescriptor = usagePageVendorDevice.Concat(hidReportDescriptor.Skip(4)).ToList();

            // add size of patched descriptor
            patchedRecord.AddRange(BitConverter.GetBytes((ushort)patchedHidReportDescriptor.Count()).Reverse());

            // add patched descriptor
            patchedRecord.AddRange(patchedHidReportDescriptor);

            // add remaining record segments
            patchedRecord.AddRange(input.Skip(dataBufferValueIndex + hidReportDescriptor.Length));

            output = patchedRecord.ToArray();

            return true;
        }
        catch (InvalidOperationException)
        {
            output = null;
            return false;
        }
    }
}