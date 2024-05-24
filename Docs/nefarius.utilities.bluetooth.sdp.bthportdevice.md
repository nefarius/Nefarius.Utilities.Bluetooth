# BthPortDevice

Namespace: Nefarius.Utilities.Bluetooth.SDP

Represents a device entry of BTHPORT.SYS

```csharp
public sealed class BthPortDevice
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [BthPortDevice](./nefarius.utilities.bluetooth.sdp.bthportdevice.md)

## Properties

### <a id="properties-cachedservices"/>**CachedServices**

Gets the cached service records blob.

```csharp
public Byte[] CachedServices { get; set; }
```

#### Property Value

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### <a id="properties-iscachedservicespatched"/>**IsCachedServicesPatched**

True if [BthPortDevice.CachedServices](./nefarius.utilities.bluetooth.sdp.bthportdevice.md#cachedservices) has been altered, false otherwise.

```csharp
public bool IsCachedServicesPatched { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### <a id="properties-originalcachedservices"/>**OriginalCachedServices**

Gets the unmodified cached service records blob.

```csharp
public Byte[] OriginalCachedServices { get; }
```

#### Property Value

[Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### <a id="properties-remoteaddress"/>**RemoteAddress**

The remote device MAC address.

```csharp
public PhysicalAddress RemoteAddress { get; }
```

#### Property Value

PhysicalAddress<br>

## Methods

### <a id="methods-deleteoriginalcachedservices"/>**DeleteOriginalCachedServices()**

Delete the backup record blob.

```csharp
public void DeleteOriginalCachedServices()
```

### <a id="methods-tostring"/>**ToString()**

```csharp
public string ToString()
```

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)
