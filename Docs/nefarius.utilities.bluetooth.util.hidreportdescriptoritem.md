# HidReportDescriptorItem

Namespace: Nefarius.Utilities.Bluetooth.Util

Represents the smallest possible entry in a HID report descriptor.

```csharp
public sealed class HidReportDescriptorItem : System.EventArgs
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [EventArgs](https://docs.microsoft.com/en-us/dotnet/api/system.eventargs) → [HidReportDescriptorItem](./nefarius.utilities.bluetooth.util.hidreportdescriptoritem.md)

## Properties

### <a id="properties-index"/>**Index**

The (zero-based) index of the location of this item in the descriptor buffer.

```csharp
public int Index { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### <a id="properties-isusagepage"/>**IsUsagePage**

True if this item is a Usage Page, false otherwise.

```csharp
public bool IsUsagePage { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### <a id="properties-itemdata"/>**ItemData**

The data/value component of this item.

```csharp
public int ItemData { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

### <a id="properties-itemsize"/>**ItemSize**

The size in bytes of this item.

```csharp
public byte ItemSize { get; }
```

#### Property Value

[Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### <a id="properties-itemtag"/>**ItemTag**

The type of this item.

```csharp
public byte ItemTag { get; }
```

#### Property Value

[Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>

### <a id="properties-stopparsing"/>**StopParsing**

If set to true by the event handler, the parser will stop its work after the event handler has finished.

```csharp
public bool StopParsing { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
