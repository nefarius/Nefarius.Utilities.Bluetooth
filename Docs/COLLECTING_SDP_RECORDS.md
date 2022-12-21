# Exporting SDP records

## Find your Controllers MAC address

Press `Win + X` and open **Device Manager**:

![RMvLLE1Kvg.png](images/RMvLLE1Kvg.png)

Expand the `Bluetooth` node and look for your controller entry (in this example it's `Wireless Controller`) and right-click, open **Properties**:

![QkYswUeI9t.png](images/QkYswUeI9t.png)

In the upcoming dialog navigate to `Details` tab and select the property `Bluetooth device address`:

![YjPkM2S0eu.png](images/YjPkM2S0eu.png)

This value is required for the next step.

## Find and export SDP records

Press `Win + R` and open `regedit`. Then navigate to `Computer\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\BTHPORT\Parameters\Devices`, identify the sub-key with the address noted in the previous step, right-click on `CachedServices` and select **Export**:

![TBYsCVY9dQ.png](images/TBYsCVY9dQ.png)

Store the resulting `.reg` file at a location of your choice and share it with @nefarius 😃
