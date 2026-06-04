# RemoteDevice

Namespace: Nefarius.Utilities.Bluetooth

Describes a remote wireless device.

```csharp
public sealed class RemoteDevice : System.IEquatable<Nefarius.Utilities.Bluetooth.RemoteDevice>
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [RemoteDevice](./nefarius.utilities.bluetooth.remotedevice.md)<br>
Implements [IEquatable](https://learn.microsoft.com/dotnet/api/system.iequatable-1)<[RemoteDevice](./nefarius.utilities.bluetooth.remotedevice.md)>

## Properties

### <a id="properties-address"/>**Address**

Gets the unique remote device address.

```csharp
public PhysicalAddress Address { get; }
```

#### Property Value

[PhysicalAddress](https://learn.microsoft.com/dotnet/api/system.net.networkinformation.physicaladdress)<br>

### <a id="properties-name"/>**Name**

Gets the reported remote device name.

```csharp
public string Name { get; }
```

#### Property Value

[String](https://learn.microsoft.com/dotnet/api/system.string)<br>

## Methods

### <a id="methods-equals"/>**Equals(RemoteDevice)**

```csharp
public bool Equals(RemoteDevice other)
```

#### Parameters

`other` [RemoteDevice](./nefarius.utilities.bluetooth.remotedevice.md)<br>

#### Returns

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="methods-equals"/>**Equals(Object)**

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://learn.microsoft.com/dotnet/api/system.object)<br>

#### Returns

[Boolean](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="methods-gethashcode"/>**GetHashCode()**

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="methods-tostring"/>**ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://learn.microsoft.com/dotnet/api/system.string)
