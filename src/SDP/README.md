# SDP Folder

## `BthPort` class

Convenience wrapper around `SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices` and other `BTHPORT.SYS`
-related actions and properties.

## `BthPortDevice` class

Convenience wrapper around a single entity within `SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices` which
represent paired remote devices and some of their more interesting properties.

## `SdpPatcher` class

Currently only holds one primary method `AlterHidDeviceToVenderDefined` with the purpose of parsing a
`SDP_ATTRIB_HID_DESCRIPTOR_LIST` binary blob and modify the embedded HID Report Descriptor from Gamepad or Joystick to
vendor Defined device usages. 
