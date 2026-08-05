# Pattern

Namespace: Nefarius.Utilities.Bluetooth.Util

Utility class to perform pattern matching/finding in byte arrays.

```csharp
public static class Pattern
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [Pattern](./nefarius.utilities.bluetooth.util.pattern.md)

## Methods

### <a id="methods-find"/>**Find(ReadOnlySpan&lt;Byte&gt;, String, ref Int32)**

Attempts to find the first occurrence of the provided pattern.

```csharp
public static bool Find(ReadOnlySpan<Byte> input, string pattern, ref Int32 offset)
```

#### Parameters

`input` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan-1)<[Byte](https://learn.microsoft.com/dotnet/api/system.byte)><br>
The array to search in.

`pattern` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The pattern to search for. Use ? or ?? as placeholders for variable content.

`offset` [Int32&](https://learn.microsoft.com/dotnet/api/system.int32&)<br>
The zero-based index, if found or -1 otherwise.

#### Returns

True if the pattern was found, false otherwise.

### <a id="methods-findall"/>**FindAll(ReadOnlySpan&lt;Byte&gt;, String, ref IEnumerable`1)**

Attempts to find all occurrences of the provided pattern.

```csharp
public static void FindAll(ReadOnlySpan<Byte> input, string pattern, ref IEnumerable`1 indexes)
```

#### Parameters

`input` [ReadOnlySpan](https://learn.microsoft.com/dotnet/api/system.readonlyspan-1)<[Byte](https://learn.microsoft.com/dotnet/api/system.byte)><br>
The array to search in.

`pattern` [String](https://learn.microsoft.com/dotnet/api/system.string)<br>
The pattern to search for. Use ? or ?? as placeholders for variable content.

`indexes` [IEnumerable`1&](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable`1[[system.int32, system.private.corelib, version=10.0.0.0, culture=neutral, publickeytoken=7cec85d7bea7798e]]&)<br>
A list of offsets where the pattern is found in the provided input array.
