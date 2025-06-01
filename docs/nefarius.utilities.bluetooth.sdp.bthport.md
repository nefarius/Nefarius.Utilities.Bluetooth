# BthPort

Namespace: Nefarius.Utilities.Bluetooth.SDP

Wrapper around BTHPORT.SYS registry structure.

```csharp
public static class BthPort
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BthPort](./nefarius.utilities.bluetooth.sdp.bthport.md)

**Remarks:**

Use of this class requires elevated privileges.

## Properties

### <a id="properties-deviceinterface"/>**DeviceInterface**

Device interface GUID.

```csharp
public static Guid DeviceInterface { get; }
```

#### Property Value

[Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>

### <a id="properties-devices"/>**Devices**

Gets a collection of paired HID devices.

```csharp
public static IEnumerable<BthPortDevice> Devices { get; }
```

#### Property Value

[IEnumerable&lt;BthPortDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

## Methods

### <a id="methods-findvaluenamecontainingrecords"/>**FindValueNameContainingRecords(RegistryKey)**

```csharp
internal static string FindValueNameContainingRecords(RegistryKey cachedServices)
```

#### Parameters

`cachedServices` RegistryKey<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)
