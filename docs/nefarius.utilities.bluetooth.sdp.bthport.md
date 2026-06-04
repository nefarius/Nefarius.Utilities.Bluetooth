# BthPort

Namespace: Nefarius.Utilities.Bluetooth.SDP

Wrapper around BTHPORT.SYS registry structure.

```csharp
public static class BthPort
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [BthPort](./nefarius.utilities.bluetooth.sdp.bthport.md)

**Remarks:**

Use of this class requires elevated privileges.

## Properties

### <a id="properties-deviceinterface"/>**DeviceInterface**

Device interface GUID.

```csharp
public static Guid DeviceInterface { get; }
```

#### Property Value

[Guid](https://learn.microsoft.com/dotnet/api/system.guid)<br>

### <a id="properties-devices"/>**Devices**

Gets a collection of paired HID devices.

```csharp
public static IEnumerable<BthPortDevice> Devices { get; }
```

#### Property Value

[IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1)<[BthPortDevice](./nefarius.utilities.bluetooth.sdp.bthportdevice.md)><br>

## Methods

### <a id="methods-findvaluenamecontainingrecords"/>**FindValueNameContainingRecords(RegistryKey)**

```csharp
internal static string FindValueNameContainingRecords(RegistryKey cachedServices)
```

#### Parameters

`cachedServices` [RegistryKey](https://learn.microsoft.com/dotnet/api/microsoft.win32.registrykey)<br>

#### Returns

[String](https://learn.microsoft.com/dotnet/api/system.string)
