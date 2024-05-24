# HostRadio

Namespace: Nefarius.Utilities.Bluetooth

Represents a Bluetooth Host Radio.

```csharp
public sealed class HostRadio : System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [HostRadio](./nefarius.utilities.bluetooth.hostradio.md)<br>
Implements [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

**Remarks:**

Windows currently only supports one exclusive online Bluetooth host radio active at the same time. This class
 can be extended in the future, should this limit ever get lifted.

## Properties

### <a id="properties-alldevices"/>**AllDevices**

Gets all remote devices.

```csharp
public IEnumerable<RemoteDevice> AllDevices { get; }
```

#### Property Value

[IEnumerable&lt;RemoteDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-authenticateddevices"/>**AuthenticatedDevices**

Gets all authenticated devices.

```csharp
public IEnumerable<RemoteDevice> AuthenticatedDevices { get; }
```

#### Property Value

[IEnumerable&lt;RemoteDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-connecteddevices"/>**ConnectedDevices**

Gets all connected devices.

```csharp
public IEnumerable<RemoteDevice> ConnectedDevices { get; }
```

#### Property Value

[IEnumerable&lt;RemoteDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-deviceinterface"/>**DeviceInterface**

Device Interface GUID.

```csharp
public static Guid DeviceInterface { get; }
```

#### Property Value

[Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>

### <a id="properties-humaninterfacedeviceserviceclassuuid"/>**HumanInterfaceDeviceServiceClassUuid**

HID Service Class GUID.

```csharp
public static Guid HumanInterfaceDeviceServiceClassUuid { get; }
```

#### Property Value

[Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>

### <a id="properties-isavailable"/>**IsAvailable**

Gets whether a host radio is available (and can be enabled).

```csharp
public static bool IsAvailable { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### <a id="properties-isenabled"/>**IsEnabled**

Gets whether a host radio is enabled.

```csharp
public static bool IsEnabled { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### <a id="properties-isoperable"/>**IsOperable**

Gets whether a host radio is available and enabled.

```csharp
public static bool IsOperable { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### <a id="properties-remembereddevices"/>**RememberedDevices**

Gets all remembered devices.

```csharp
public IEnumerable<RemoteDevice> RememberedDevices { get; }
```

#### Property Value

[IEnumerable&lt;RemoteDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-unknowndevices"/>**UnknownDevices**

Gets all unknown devices.

```csharp
public IEnumerable<RemoteDevice> UnknownDevices { get; }
```

#### Property Value

[IEnumerable&lt;RemoteDevice&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

## Constructors

### <a id="constructors-.ctor"/>**HostRadio(Boolean)**

Creates a new instance.

```csharp
public HostRadio(bool autoEnable)
```

#### Parameters

`autoEnable` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True to automatically enable the radio if currently disabled, false will throw an exception.

#### Exceptions

[HostRadioException](./nefarius.utilities.bluetooth.exceptions.hostradioexception.md)<br>
Radio handle access has failed.

## Methods

### <a id="methods-disableradio"/>**DisableRadio()**

Disables the host radio.

```csharp
public void DisableRadio()
```

#### Exceptions

[HostRadioException](./nefarius.utilities.bluetooth.exceptions.hostradioexception.md)<br>

### <a id="methods-disableservice"/>**DisableService(Guid, String)**

Disables advertising a specified service.

```csharp
public void DisableService(Guid serviceGuid, string serviceName)
```

#### Parameters

`serviceGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The GUID of the service to expose. This should match the GUID in the server-side INF file.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The service name.

#### Exceptions

[BluetoothServiceException](./nefarius.utilities.bluetooth.exceptions.bluetoothserviceexception.md)<br>

[AdjustProcessPrivilegesException](./nefarius.utilities.bluetooth.exceptions.adjustprocessprivilegesexception.md)<br>

**Remarks:**

This method requires administrative privileges.

### <a id="methods-disconnectremotedevice"/>**DisconnectRemoteDevice(String)**

Instruct host radio to disconnect a given remote device.

```csharp
public void DisconnectRemoteDevice(string address)
```

#### Parameters

`address` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A parseable MAC address string.

**Remarks:**

See
 https://learn.microsoft.com/en-us/dotnet/api/system.net.networkinformation.physicaladdress.parse?view=net-7.0 for
 valid string formats.

### <a id="methods-disconnectremotedevice"/>**DisconnectRemoteDevice(RemoteDevice)**

Instruct host radio to disconnect a given remote device.

```csharp
public void DisconnectRemoteDevice(RemoteDevice device)
```

#### Parameters

`device` [RemoteDevice](./nefarius.utilities.bluetooth.remotedevice.md)<br>
The [RemoteDevice](./nefarius.utilities.bluetooth.remotedevice.md) to disconnect.

### <a id="methods-disconnectremotedevice"/>**DisconnectRemoteDevice(PhysicalAddress)**

Instruct host radio to disconnect a given remote device.

```csharp
public void DisconnectRemoteDevice(PhysicalAddress device)
```

#### Parameters

`device` PhysicalAddress<br>
The MAC address of the remote device.

### <a id="methods-dispose"/>**Dispose()**

```csharp
public void Dispose()
```

### <a id="methods-enableradio"/>**EnableRadio()**

Enables the host radio.

```csharp
public void EnableRadio()
```

#### Exceptions

[HostRadioException](./nefarius.utilities.bluetooth.exceptions.hostradioexception.md)<br>

### <a id="methods-enableservice"/>**EnableService(Guid, String)**

Enables advertising a specified service.

```csharp
public void EnableService(Guid serviceGuid, string serviceName)
```

#### Parameters

`serviceGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The GUID of the service to expose. This should match the GUID in the server-side INF file.

`serviceName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The service name.

#### Exceptions

[BluetoothServiceException](./nefarius.utilities.bluetooth.exceptions.bluetoothserviceexception.md)<br>

[AdjustProcessPrivilegesException](./nefarius.utilities.bluetooth.exceptions.adjustprocessprivilegesexception.md)<br>

**Remarks:**

This method requires administrative privileges.

### <a id="methods-getservicestatefordevice"/>**GetServiceStateForDevice(PhysicalAddress, Guid, ref Boolean)**

Gets the state of a specified service.

```csharp
public bool GetServiceStateForDevice(PhysicalAddress address, Guid serviceGuid, ref Boolean enabled)
```

#### Parameters

`address` PhysicalAddress<br>
The remote device address.

`serviceGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The service GUID.

`enabled` [Boolean&](https://docs.microsoft.com/en-us/dotnet/api/system.boolean&)<br>
True if the service is enabled, false otherwise.

#### Returns

True if the supplied device was found, false otherwise.

#### Exceptions

[HostRadioException](./nefarius.utilities.bluetooth.exceptions.hostradioexception.md)<br>

### <a id="methods-restartradio"/>**RestartRadio()**

Restarts the host radio.

```csharp
public void RestartRadio()
```

### <a id="methods-setservicestatefordevice"/>**SetServiceStateForDevice(PhysicalAddress, Guid, Boolean)**

Sets the state of a specified service to either enabled or disabled.

```csharp
public void SetServiceStateForDevice(PhysicalAddress address, Guid serviceGuid, bool enabled)
```

#### Parameters

`address` PhysicalAddress<br>
The remote device address.

`serviceGuid` [Guid](https://docs.microsoft.com/en-us/dotnet/api/system.guid)<br>
The service GUID.

`enabled` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True to set to enabled, false to disable.

#### Exceptions

[HostRadioException](./nefarius.utilities.bluetooth.exceptions.hostradioexception.md)<br>
