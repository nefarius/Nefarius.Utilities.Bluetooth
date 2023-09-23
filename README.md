<img src="assets/NSS-128x128.png" align="right" />

# Nefarius.Utilities.Bluetooth

[![.NET](https://github.com/nefarius/Nefarius.Utilities.Bluetooth/actions/workflows/build.yml/badge.svg)](https://github.com/nefarius/Nefarius.Utilities.Bluetooth/actions/workflows/build.yml) ![Requirements](https://img.shields.io/badge/Requires-.NET%206-blue.svg) ![Requirements](https://img.shields.io/badge/Requires-.NET%20Standard%202.0-blue.svg) [![Nuget](https://img.shields.io/nuget/v/Nefarius.Utilities.Bluetooth)](https://www.nuget.org/packages/Nefarius.Utilities.Bluetooth/) [![Nuget](https://img.shields.io/nuget/dt/Nefarius.Utilities.Bluetooth)](https://www.nuget.org/packages/Nefarius.Utilities.Bluetooth/)

Utility library for unconventional Bluetooth tasks on Windows.

Work in progress, use with care 🔥

## About

This is a collection of utility classes using undocumented Windows APIs to achieve wireless greatness! Ever needed a simple method of enabling or disabling Bluetooth without all that UWP and Store App nonsense? Wanna dive into modifying SDP records on your machine? This ever growing library will provide without any bloated dependencies! Enjoy and use responsibly! 😃

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

### Disconnect a remote device

```csharp
using var radio = new HostRadio();
radio.DisconnectRemoteDevice("MAC address");
```

## 3rd party credits

- [USB Descriptor and Request Parser](http://eleccelerator.com/usbdescreqparser/)
- [Display Filter Reference: Bluetooth SDP Protocol](https://www.wireshark.org/docs/dfref/b/btsdp.html)
- [C# Pattern Scan(Array Of Byte Scan) Class](https://zpackdev.blogspot.com/2016/10/c-pattern-scanarray-of-byte-scan-class.html)
- [Simple C# Pattern Scan](https://guidedhacking.com/threads/simple-c-pattern-scan.13981/)
- [USB-HID-Report-Parser](https://github.com/uint32tMnstr/USB-HID-Report-Parser)
- [BoyerMoore](https://github.com/TheAlgorithms/C-Sharp/blob/master/Algorithms/Strings/BoyerMoore.cs)
- [Generic Boyer–Moore–Horspool algorithm in C# .NET](https://swimburger.net/blog/dotnet/generic-boyer-moore-horspool-algorithm-in-csharp-dotnet)
- [BluetoothHelper.cs](https://github.com/ViGEm/DsHidMini/blob/37f46a65a9fa12637aff9ca658c7eb8543f28883/DSHMC/Util/BluetoothHelper.cs)
