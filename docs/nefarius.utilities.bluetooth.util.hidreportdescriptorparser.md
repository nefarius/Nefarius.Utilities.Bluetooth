# HidReportDescriptorParser

Namespace: Nefarius.Utilities.Bluetooth.Util

HID Report Descriptor parser.

```csharp
public class HidReportDescriptorParser
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [HidReportDescriptorParser](./nefarius.utilities.bluetooth.util.hidreportdescriptorparser.md)

**Remarks:**

Source: https://github.com/uint32tMnstr/USB-HID-Report-Parser

## Constructors

### <a id="constructors-.ctor"/>**HidReportDescriptorParser()**

```csharp
public HidReportDescriptorParser()
```

## Methods

### <a id="methods-parse"/>**Parse(Byte[])**

Parses a provided descriptor buffer into individual items.

```csharp
public bool Parse(Byte[] descriptor)
```

#### Parameters

`descriptor` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br>
The raw descriptor buffer.

#### Returns

True if the full report could be parsed, false if it got interrupted by the user or by error.

## Events

### <a id="events-globalitemparsed"/>**GlobalItemParsed**

Raised when a global item has been parsed.

```csharp
public event Action<HidReportDescriptorItem> GlobalItemParsed;
```

### <a id="events-localitemparsed"/>**LocalItemParsed**

Raised when a local item has been parsed.

```csharp
public event Action<HidReportDescriptorItem> LocalItemParsed;
```

### <a id="events-mainitemparsed"/>**MainItemParsed**

Raised when a main item has been parsed.

```csharp
public event Action<HidReportDescriptorItem> MainItemParsed;
```
