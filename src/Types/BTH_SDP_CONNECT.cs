using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Nefarius.Utilities.Bluetooth.Types;

[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
internal struct BTH_SDP_CONNECT
{
#pragma warning disable CS0169
#pragma warning disable IDE1006
    internal UInt64 bthAddress;
    internal UInt32 fSdpConnect;
    internal UInt64 hConnection;
    internal byte requestTimeout;
#pragma warning restore CS0169
#pragma warning restore IDE1006
}