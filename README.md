<img src="assets/NSS-128x128.png" align="right" />

# Nefarius.Utilities.Bluetooth

[![Build status](https://ci.appveyor.com/api/projects/status/7rs1an8bmwnm2bw9/branch/master?svg=true)](https://ci.appveyor.com/project/nefarius/nefarius-utilities-bluetooth/branch/master) ![Requirements](https://img.shields.io/badge/Requires-.NET%206-blue.svg) ![Requirements](https://img.shields.io/badge/Requires-.NET%20Standard%202.0-blue.svg) [![Nuget](https://img.shields.io/nuget/v/Nefarius.Utilities.Bluetooth)](https://www.nuget.org/packages/Nefarius.Utilities.Bluetooth/) [![Nuget](https://img.shields.io/nuget/dt/Nefarius.Utilities.Bluetooth)](https://www.nuget.org/packages/Nefarius.Utilities.Bluetooth/)

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

## 3rd party credits

- [USB Descriptor and Request Parser](http://eleccelerator.com/usbdescreqparser/)
- [Display Filter Reference: Bluetooth SDP Protocol](https://www.wireshark.org/docs/dfref/b/btsdp.html)
- [C# Pattern Scan(Array Of Byte Scan) Class](https://zpackdev.blogspot.com/2016/10/c-pattern-scanarray-of-byte-scan-class.html)
- [Simple C# Pattern Scan](https://guidedhacking.com/threads/simple-c-pattern-scan.13981/)
- [USB-HID-Report-Parser](https://github.com/uint32tMnstr/USB-HID-Report-Parser)
- [BoyerMoore](https://github.com/TheAlgorithms/C-Sharp/blob/master/Algorithms/Strings/BoyerMoore.cs)
