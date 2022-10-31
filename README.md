<img src="assets/NSS-128x128.png" align="right" />

# Nefarius.Utilities.Bluetooth

[![Build status](https://ci.appveyor.com/api/projects/status/7rs1an8bmwnm2bw9/branch/master?svg=true)](https://ci.appveyor.com/project/nefarius/nefarius-utilities-bluetooth/branch/master) 

Utility library for unconventional Bluetooth tasks on Windows.

Work in progress, do not use 🔥

## Examples

### Turn Bluetooth On, Off or Restart it

Turn on:

```csharp
using var radio = new HostRadio();
radio.EnableRadio();
```

Turn off:

```csharp
using var radio = new HostRadio();
radio.DisableRadio();
```

Restart/reload:

```csharp
using var radio = new HostRadio();
radio.RestartRadio();
```
