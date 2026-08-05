# SdpPatcher

Namespace: Nefarius.Utilities.Bluetooth.SDP

Service Discovery Record Patching Utility.

```csharp
public static class SdpPatcher
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [SdpPatcher](./nefarius.utilities.bluetooth.sdp.sdppatcher.md)

## Methods

### <a id="methods-alterhiddevicetovendordefined"/>**AlterHidDeviceToVendorDefined(Byte[], ref Byte[])**

Attempts to find an SDP_ATTRIB_HID_DESCRIPTOR_LIST attribute and patches the HID Report Descriptor to a Vendor
 Defined device on success.

```csharp
public static bool AlterHidDeviceToVendorDefined(Byte[] input, ref Byte[] output)
```

#### Parameters

`input` [Byte[]](https://learn.microsoft.com/dotnet/api/system.byte[])<br>
The original record array.

`output` [Byte[]&](https://learn.microsoft.com/dotnet/api/system.byte[]&)<br>
The patched record array.

#### Returns

True if detection and patching were successful, false otherwise.
