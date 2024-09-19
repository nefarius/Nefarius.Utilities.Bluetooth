using Nefarius.Utilities.Bluetooth;
using Nefarius.Utilities.Bluetooth.SDP;
using Nefarius.Utilities.Registry;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test, Order(1)]
    public void TestTurnOff()
    {
        using HostRadio radio = new();
        
        radio.DisableRadio();
    }
    
    [Test, Order(2)]
    public void TestTurnOn()
    {
        using HostRadio radio = new();
        
        radio.EnableRadio();
    }

    /// <summary>
    ///     For this test to succeed make sure the radio is enabled.
    /// </summary>
    [Test, Order(3)]
    public void TestAvailabilityIfOn()
    {
        Assert.Multiple(() =>
        {
            Assert.That(HostRadio.IsAvailable, Is.True);
            Assert.That(HostRadio.IsEnabled, Is.True);
            Assert.That(HostRadio.IsOperable, Is.True);
        });
    }
    
    [Test, Order(4)]
    public void TestEnableService()
    {
        using HostRadio radio = new();

        Guid serviceGuid = Guid.Parse("{1cb831ea-79cd-4508-b0fc-85f7c85ae8e0}");
        string serviceName = "BthPS3Service";
        
        radio.EnableService(serviceGuid, serviceName);
    }

    [Test, Order(5)]
    public void TestDisableService()
    {
        using HostRadio radio = new();

        Guid serviceGuid = Guid.Parse("{1cb831ea-79cd-4508-b0fc-85f7c85ae8e0}");
        string serviceName = "BthPS3Service";

        radio.DisableService(serviceGuid, serviceName);
    }

    //[Test]
    public void Test2()
    {
        string[] testFiles = Directory.GetFiles(
            @"D:\Development\GitHub\Nefarius.Utilities.Bluetooth\Dumps",
            "*.reg",
            SearchOption.AllDirectories
        );

        foreach (string testFile in testFiles)
        {
            RegFile file = new(testFile);

            KeyValuePair<string, Dictionary<string, RegValue>> cachedEntries = file.RegValues.First();

            Dictionary<string, RegValueBinary> binValues = cachedEntries.Value
                .Where(v => v.Value.Type == RegValueType.Binary)
                .ToDictionary(pair => pair.Key, pair => (RegValueBinary)pair.Value);

            Assert.That(binValues, Is.Not.Empty);

            KeyValuePair<string, RegValueBinary> recordEntry =
                binValues.FirstOrDefault(pair => pair.Value.Value.First() == 0x36);

            byte[] recordBlob = recordEntry.Value.Value.ToArray();
        }
    }

    //[Test]
    public void Test1()
    {
        using HostRadio radio = new();

        List<RemoteDevice> a = radio.AllDevices.ToList();

        foreach (BthPortDevice? device in BthPort.Devices.ToList())
        {
            radio.GetServiceStateForDevice(device.RemoteAddress, HostRadio.HumanInterfaceDeviceServiceClassUuid,
                out bool state);
        }

        /*
        var descriptor = new byte[]
        {
            0x05, 0x01, // Usage Page (Generic Desktop Ctrls)
            0x09, 0x05, // Usage (Game Pad)
            0xA1, 0x01, // Collection (Application)
            0x85, 0x01, //   Report ID (1)
            0x09, 0x30, //   Usage (X)
            0x09, 0x31, //   Usage (Y)
            0x09, 0x32, //   Usage (Z)
            0x09, 0x35, //   Usage (Rz)
            0x15, 0x00, //   Logical Minimum (0)
            0x26, 0xFF, 0x00, //   Logical Maximum (255)
            0x75, 0x08, //   Report Size (8)
            0x95, 0x04, //   Report Count (4)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x39, //   Usage (Hat switch)
            0x15, 0x00, //   Logical Minimum (0)
            0x25, 0x07, //   Logical Maximum (7)
            0x75, 0x04, //   Report Size (4)
            0x95, 0x01, //   Report Count (1)
            0x81, 0x42, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,Null State)
            0x05, 0x09, //   Usage Page (Button)
            0x19, 0x01, //   Usage Minimum (0x01)
            0x29, 0x0E, //   Usage Maximum (0x0E)
            0x15, 0x00, //   Logical Minimum (0)
            0x25, 0x01, //   Logical Maximum (1)
            0x75, 0x01, //   Report Size (1)
            0x95, 0x0E, //   Report Count (14)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x75, 0x06, //   Report Size (6)
            0x95, 0x01, //   Report Count (1)
            0x81, 0x01, //   Input (Const,Array,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x05, 0x01, //   Usage Page (Generic Desktop Ctrls)
            0x09, 0x33, //   Usage (Rx)
            0x09, 0x34, //   Usage (Ry)
            0x15, 0x00, //   Logical Minimum (0)
            0x26, 0xFF, 0x00, //   Logical Maximum (255)
            0x75, 0x08, //   Report Size (8)
            0x95, 0x02, //   Report Count (2)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x06, 0x04, 0xFF, //   Usage Page (Vendor Defined 0xFF04)
            0x85, 0x02, //   Report ID (2)
            0x09, 0x24, //   Usage (0x24)
            0x95, 0x24, //   Report Count (36)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xA3, //   Report ID (-93)
            0x09, 0x25, //   Usage (0x25)
            0x95, 0x30, //   Report Count (48)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x05, //   Report ID (5)
            0x09, 0x26, //   Usage (0x26)
            0x95, 0x28, //   Report Count (40)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x06, //   Report ID (6)
            0x09, 0x27, //   Usage (0x27)
            0x95, 0x34, //   Report Count (52)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x07, //   Report ID (7)
            0x09, 0x28, //   Usage (0x28)
            0x95, 0x30, //   Report Count (48)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x08, //   Report ID (8)
            0x09, 0x29, //   Usage (0x29)
            0x95, 0x2F, //   Report Count (47)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x09, //   Report ID (9)
            0x09, 0x2A, //   Usage (0x2A)
            0x95, 0x13, //   Report Count (19)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x06, 0x03, 0xFF, //   Usage Page (Vendor Defined 0xFF03)
            0x85, 0x03, //   Report ID (3)
            0x09, 0x21, //   Usage (0x21)
            0x95, 0x26, //   Report Count (38)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x04, //   Report ID (4)
            0x09, 0x22, //   Usage (0x22)
            0x95, 0x2E, //   Report Count (46)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xF0, //   Report ID (-16)
            0x09, 0x47, //   Usage (0x47)
            0x95, 0x3F, //   Report Count (63)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xF1, //   Report ID (-15)
            0x09, 0x48, //   Usage (0x48)
            0x95, 0x3F, //   Report Count (63)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xF2, //   Report ID (-14)
            0x09, 0x49, //   Usage (0x49)
            0x95, 0x0F, //   Report Count (15)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x06, 0x00, 0xFF, //   Usage Page (Vendor Defined 0xFF00)
            0x85, 0x11, //   Report ID (17)
            0x09, 0x20, //   Usage (0x20)
            0x15, 0x00, //   Logical Minimum (0)
            0x26, 0xFF, 0x00, //   Logical Maximum (255)
            0x75, 0x08, //   Report Size (8)
            0x95, 0x4D, //   Report Count (77)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x21, //   Usage (0x21)
            0x91, 0x02, //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x12, //   Report ID (18)
            0x09, 0x22, //   Usage (0x22)
            0x95, 0x8D, //   Report Count (-115)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x23, //   Usage (0x23)
            0x91, 0x02, //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x13, //   Report ID (19)
            0x09, 0x24, //   Usage (0x24)
            0x95, 0xCD, //   Report Count (-51)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x25, //   Usage (0x25)
            0x91, 0x02, //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x14, //   Report ID (20)
            0x09, 0x26, //   Usage (0x26)
            0x96, 0x0D, 0x01, //   Report Count (269)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x27, //   Usage (0x27)
            0x91, 0x02, //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x15, //   Report ID (21)
            0x09, 0x28, //   Usage (0x28)
            0x96, 0x4D, 0x01, //   Report Count (333)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x29, //   Usage (0x29)
            0x91, 0x02, //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x16, //   Report ID (22)
            0x09, 0x2A, //   Usage (0x2A)
            0x96, 0x8D, 0x01, //   Report Count (397)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x2B, //   Usage (0x2B)
            0x91, 0x02, //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x17, //   Report ID (23)
            0x09, 0x2C, //   Usage (0x2C)
            0x96, 0xCD, 0x01, //   Report Count (461)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x2D, //   Usage (0x2D)
            0x91, 0x02, //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x18, //   Report ID (24)
            0x09, 0x2E, //   Usage (0x2E)
            0x96, 0x0D, 0x02, //   Report Count (525)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x2F, //   Usage (0x2F)
            0x91, 0x02, //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x19, //   Report ID (25)
            0x09, 0x30, //   Usage (0x30)
            0x96, 0x22, 0x02, //   Report Count (546)
            0x81, 0x02, //   Input (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position)
            0x09, 0x31, //   Usage (0x31)
            0x91, 0x02, //   Output (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x06, 0x80, 0xFF, //   Usage Page (Vendor Defined 0xFF80)
            0x85, 0x82, //   Report ID (-126)
            0x09, 0x22, //   Usage (0x22)
            0x95, 0x3F, //   Report Count (63)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x83, //   Report ID (-125)
            0x09, 0x23, //   Usage (0x23)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x84, //   Report ID (-124)
            0x09, 0x24, //   Usage (0x24)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x90, //   Report ID (-112)
            0x09, 0x30, //   Usage (0x30)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x91, //   Report ID (-111)
            0x09, 0x31, //   Usage (0x31)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x92, //   Report ID (-110)
            0x09, 0x32, //   Usage (0x32)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x93, //   Report ID (-109)
            0x09, 0x33, //   Usage (0x33)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0x94, //   Report ID (-108)
            0x09, 0x34, //   Usage (0x34)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xA0, //   Report ID (-96)
            0x09, 0x40, //   Usage (0x40)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xA4, //   Report ID (-92)
            0x09, 0x44, //   Usage (0x44)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xA7, //   Report ID (-89)
            0x09, 0x45, //   Usage (0x45)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xA8, //   Report ID (-88)
            0x09, 0x45, //   Usage (0x45)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xA9, //   Report ID (-87)
            0x09, 0x45, //   Usage (0x45)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xAA, //   Report ID (-86)
            0x09, 0x45, //   Usage (0x45)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xAB, //   Report ID (-85)
            0x09, 0x45, //   Usage (0x45)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xAC, //   Report ID (-84)
            0x09, 0x45, //   Usage (0x45)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xAD, //   Report ID (-83)
            0x09, 0x45, //   Usage (0x45)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xB3, //   Report ID (-77)
            0x09, 0x45, //   Usage (0x45)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xB4, //   Report ID (-76)
            0x09, 0x46, //   Usage (0x46)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xB5, //   Report ID (-75)
            0x09, 0x47, //   Usage (0x47)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xD0, //   Report ID (-48)
            0x09, 0x40, //   Usage (0x40)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0x85, 0xD4, //   Report ID (-44)
            0x09, 0x44, //   Usage (0x44)
            0xB1, 0x02, //   Feature (Data,Var,Abs,No Wrap,Linear,Preferred State,No Null Position,Non-volatile)
            0xC0 // End Collection

            // 442 bytes

            // best guess: USB HID Report Descriptor
        };

        var parser = new HidReportDescriptorParser();

        parser.GlobalItemParsed += item =>
        {
            if (item.IsUsagePage)
            {

            }
        };

        parser.Parse(descriptor);
        */

        byte[]? buffer =
        {
            0x36, 0x02, 0xBA, 0x09, 0x00, 0x00, 0x0A, 0x00, 0x01, 0x00, 0x01, 0x09, 0x00, 0x01, 0x35, 0x03, 0x19,
            0x11, 0x24, 0x09, 0x00, 0x04, 0x35, 0x0D, 0x35, 0x06, 0x19, 0x01, 0x00, 0x09, 0x00, 0x11, 0x35, 0x03,
            0x19, 0x00, 0x11, 0x09, 0x00, 0x06, 0x35, 0x09, 0x09, 0x65, 0x6E, 0x09, 0x00, 0x6A, 0x09, 0x01, 0x00,
            0x09, 0x00, 0x09, 0x35, 0x08, 0x35, 0x06, 0x19, 0x11, 0x24, 0x09, 0x01, 0x00, 0x09, 0x00, 0x0D, 0x35,
            0x0F, 0x35, 0x0D, 0x35, 0x06, 0x19, 0x01, 0x00, 0x09, 0x00, 0x13, 0x35, 0x03, 0x19, 0x00, 0x11, 0x09,
            0x01, 0x00, 0x25, 0x13, 0x57, 0x69, 0x72, 0x65, 0x6C, 0x65, 0x73, 0x73, 0x20, 0x43, 0x6F, 0x6E, 0x74,
            0x72, 0x6F, 0x6C, 0x6C, 0x65, 0x72, 0x09, 0x01, 0x01, 0x25, 0x0F, 0x47, 0x61, 0x6D, 0x65, 0x20, 0x43,
            0x6F, 0x6E, 0x74, 0x72, 0x6F, 0x6C, 0x6C, 0x65, 0x72, 0x09, 0x01, 0x02, 0x25, 0x1E, 0x53, 0x6F, 0x6E,
            0x79, 0x20, 0x49, 0x6E, 0x74, 0x65, 0x72, 0x61, 0x63, 0x74, 0x69, 0x76, 0x65, 0x20, 0x45, 0x6E, 0x74,
            0x65, 0x72, 0x74, 0x61, 0x69, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x09, 0x02, 0x00, 0x09, 0x01, 0x00, 0x09,
            0x02, 0x01, 0x09, 0x01, 0x11, 0x09, 0x02, 0x02, 0x08, 0x08, 0x09, 0x02, 0x03, 0x08, 0x00, 0x09, 0x02,
            0x04, 0x28, 0x00, 0x09, 0x02, 0x05, 0x28, 0x01, 0x09, 0x02, 0x06, 0x36, 0x01, 0xC2, 0x36, 0x01, 0xBF,
            0x08, 0x22, 0x26, 0x01, 0xBA, 0x05, 0x01, 0x09, 0x05, 0xA1, 0x01, 0x85, 0x01, 0x09, 0x30, 0x09, 0x31,
            0x09, 0x32, 0x09, 0x35, 0x15, 0x00, 0x26, 0xFF, 0x00, 0x75, 0x08, 0x95, 0x04, 0x81, 0x02, 0x09, 0x39,
            0x15, 0x00, 0x25, 0x07, 0x75, 0x04, 0x95, 0x01, 0x81, 0x42, 0x05, 0x09, 0x19, 0x01, 0x29, 0x0E, 0x15,
            0x00, 0x25, 0x01, 0x75, 0x01, 0x95, 0x0E, 0x81, 0x02, 0x75, 0x06, 0x95, 0x01, 0x81, 0x01, 0x05, 0x01,
            0x09, 0x33, 0x09, 0x34, 0x15, 0x00, 0x26, 0xFF, 0x00, 0x75, 0x08, 0x95, 0x02, 0x81, 0x02, 0x06, 0x04,
            0xFF, 0x85, 0x02, 0x09, 0x24, 0x95, 0x24, 0xB1, 0x02, 0x85, 0xA3, 0x09, 0x25, 0x95, 0x30, 0xB1, 0x02,
            0x85, 0x05, 0x09, 0x26, 0x95, 0x28, 0xB1, 0x02, 0x85, 0x06, 0x09, 0x27, 0x95, 0x34, 0xB1, 0x02, 0x85,
            0x07, 0x09, 0x28, 0x95, 0x30, 0xB1, 0x02, 0x85, 0x08, 0x09, 0x29, 0x95, 0x2F, 0xB1, 0x02, 0x85, 0x09,
            0x09, 0x2A, 0x95, 0x13, 0xB1, 0x02, 0x06, 0x03, 0xFF, 0x85, 0x03, 0x09, 0x21, 0x95, 0x26, 0xB1, 0x02,
            0x85, 0x04, 0x09, 0x22, 0x95, 0x2E, 0xB1, 0x02, 0x85, 0xF0, 0x09, 0x47, 0x95, 0x3F, 0xB1, 0x02, 0x85,
            0xF1, 0x09, 0x48, 0x95, 0x3F, 0xB1, 0x02, 0x85, 0xF2, 0x09, 0x49, 0x95, 0x0F, 0xB1, 0x02, 0x06, 0x00,
            0xFF, 0x85, 0x11, 0x09, 0x20, 0x15, 0x00, 0x26, 0xFF, 0x00, 0x75, 0x08, 0x95, 0x4D, 0x81, 0x02, 0x09,
            0x21, 0x91, 0x02, 0x85, 0x12, 0x09, 0x22, 0x95, 0x8D, 0x81, 0x02, 0x09, 0x23, 0x91, 0x02, 0x85, 0x13,
            0x09, 0x24, 0x95, 0xCD, 0x81, 0x02, 0x09, 0x25, 0x91, 0x02, 0x85, 0x14, 0x09, 0x26, 0x96, 0x0D, 0x01,
            0x81, 0x02, 0x09, 0x27, 0x91, 0x02, 0x85, 0x15, 0x09, 0x28, 0x96, 0x4D, 0x01, 0x81, 0x02, 0x09, 0x29,
            0x91, 0x02, 0x85, 0x16, 0x09, 0x2A, 0x96, 0x8D, 0x01, 0x81, 0x02, 0x09, 0x2B, 0x91, 0x02, 0x85, 0x17,
            0x09, 0x2C, 0x96, 0xCD, 0x01, 0x81, 0x02, 0x09, 0x2D, 0x91, 0x02, 0x85, 0x18, 0x09, 0x2E, 0x96, 0x0D,
            0x02, 0x81, 0x02, 0x09, 0x2F, 0x91, 0x02, 0x85, 0x19, 0x09, 0x30, 0x96, 0x22, 0x02, 0x81, 0x02, 0x09,
            0x31, 0x91, 0x02, 0x06, 0x80, 0xFF, 0x85, 0x82, 0x09, 0x22, 0x95, 0x3F, 0xB1, 0x02, 0x85, 0x83, 0x09,
            0x23, 0xB1, 0x02, 0x85, 0x84, 0x09, 0x24, 0xB1, 0x02, 0x85, 0x90, 0x09, 0x30, 0xB1, 0x02, 0x85, 0x91,
            0x09, 0x31, 0xB1, 0x02, 0x85, 0x92, 0x09, 0x32, 0xB1, 0x02, 0x85, 0x93, 0x09, 0x33, 0xB1, 0x02, 0x85,
            0x94, 0x09, 0x34, 0xB1, 0x02, 0x85, 0xA0, 0x09, 0x40, 0xB1, 0x02, 0x85, 0xA4, 0x09, 0x44, 0xB1, 0x02,
            0x85, 0xA7, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xA8, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xA9, 0x09, 0x45, 0xB1,
            0x02, 0x85, 0xAA, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xAB, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xAC, 0x09, 0x45,
            0xB1, 0x02, 0x85, 0xAD, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xB3, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xB4, 0x09,
            0x46, 0xB1, 0x02, 0x85, 0xB5, 0x09, 0x47, 0xB1, 0x02, 0x85, 0xD0, 0x09, 0x40, 0xB1, 0x02, 0x85, 0xD4,
            0x09, 0x44, 0xB1, 0x02, 0xC0, 0x09, 0x02, 0x07, 0x35, 0x08, 0x35, 0x06, 0x09, 0x04, 0x09, 0x09, 0x01,
            0x00, 0x09, 0x02, 0x08, 0x28, 0x00, 0x09, 0x02, 0x09, 0x28, 0x01, 0x09, 0x02, 0x0A, 0x28, 0x01, 0x09,
            0x02, 0x0B, 0x09, 0x01, 0x00, 0x09, 0x02, 0x0C, 0x09, 0x1F, 0x40, 0x09, 0x02, 0x0D, 0x28, 0x00, 0x09,
            0x02, 0x0E, 0x28, 0x00
        };

        byte[] altered =
        {
            0x36, 0x02, 0xBB, 0x09, 0x00, 0x00, 0x0A, 0x00, 0x01, 0x00, 0x01, 0x09, 0x00, 0x01, 0x35, 0x03, 0x19,
            0x11, 0x24, 0x09, 0x00, 0x04, 0x35, 0x0D, 0x35, 0x06, 0x19, 0x01, 0x00, 0x09, 0x00, 0x11, 0x35, 0x03,
            0x19, 0x00, 0x11, 0x09, 0x00, 0x06, 0x35, 0x09, 0x09, 0x65, 0x6E, 0x09, 0x00, 0x6A, 0x09, 0x01, 0x00,
            0x09, 0x00, 0x09, 0x35, 0x08, 0x35, 0x06, 0x19, 0x11, 0x24, 0x09, 0x01, 0x00, 0x09, 0x00, 0x0D, 0x35,
            0x0F, 0x35, 0x0D, 0x35, 0x06, 0x19, 0x01, 0x00, 0x09, 0x00, 0x13, 0x35, 0x03, 0x19, 0x00, 0x11, 0x09,
            0x01, 0x00, 0x25, 0x13, 0x57, 0x69, 0x72, 0x65, 0x6C, 0x65, 0x73, 0x73, 0x20, 0x43, 0x6F, 0x6E, 0x74,
            0x72, 0x6F, 0x6C, 0x6C, 0x65, 0x72, 0x09, 0x01, 0x01, 0x25, 0x0F, 0x47, 0x61, 0x6D, 0x65, 0x20, 0x43,
            0x6F, 0x6E, 0x74, 0x72, 0x6F, 0x6C, 0x6C, 0x65, 0x72, 0x09, 0x01, 0x02, 0x25, 0x1E, 0x53, 0x6F, 0x6E,
            0x79, 0x20, 0x49, 0x6E, 0x74, 0x65, 0x72, 0x61, 0x63, 0x74, 0x69, 0x76, 0x65, 0x20, 0x45, 0x6E, 0x74,
            0x65, 0x72, 0x74, 0x61, 0x69, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x09, 0x02, 0x00, 0x09, 0x01, 0x00, 0x09,
            0x02, 0x01, 0x09, 0x01, 0x11, 0x09, 0x02, 0x02, 0x08, 0x08, 0x09, 0x02, 0x03, 0x08, 0x00, 0x09, 0x02,
            0x04, 0x28, 0x00, 0x09, 0x02, 0x05, 0x28, 0x01, 0x09, 0x02, 0x06, 0x36, 0x01, 0xC3, 0x36, 0x01, 0xC0,
            0x08, 0x22, 0x26, 0x01, 0xBB, 0x06, 0x01, 0xFF, 0x09, 0x01, 0xA1, 0x01, 0x85, 0x01, 0x09, 0x30, 0x09,
            0x31, 0x09, 0x32, 0x09, 0x35, 0x15, 0x00, 0x26, 0xFF, 0x00, 0x75, 0x08, 0x95, 0x04, 0x81, 0x02, 0x09,
            0x39, 0x15, 0x00, 0x25, 0x07, 0x75, 0x04, 0x95, 0x01, 0x81, 0x42, 0x05, 0x09, 0x19, 0x01, 0x29, 0x0E,
            0x15, 0x00, 0x25, 0x01, 0x75, 0x01, 0x95, 0x0E, 0x81, 0x02, 0x75, 0x06, 0x95, 0x01, 0x81, 0x01, 0x05,
            0x01, 0x09, 0x33, 0x09, 0x34, 0x15, 0x00, 0x26, 0xFF, 0x00, 0x75, 0x08, 0x95, 0x02, 0x81, 0x02, 0x06,
            0x04, 0xFF, 0x85, 0x02, 0x09, 0x24, 0x95, 0x24, 0xB1, 0x02, 0x85, 0xA3, 0x09, 0x25, 0x95, 0x30, 0xB1,
            0x02, 0x85, 0x05, 0x09, 0x26, 0x95, 0x28, 0xB1, 0x02, 0x85, 0x06, 0x09, 0x27, 0x95, 0x34, 0xB1, 0x02,
            0x85, 0x07, 0x09, 0x28, 0x95, 0x30, 0xB1, 0x02, 0x85, 0x08, 0x09, 0x29, 0x95, 0x2F, 0xB1, 0x02, 0x85,
            0x09, 0x09, 0x2A, 0x95, 0x13, 0xB1, 0x02, 0x06, 0x03, 0xFF, 0x85, 0x03, 0x09, 0x21, 0x95, 0x26, 0xB1,
            0x02, 0x85, 0x04, 0x09, 0x22, 0x95, 0x2E, 0xB1, 0x02, 0x85, 0xF0, 0x09, 0x47, 0x95, 0x3F, 0xB1, 0x02,
            0x85, 0xF1, 0x09, 0x48, 0x95, 0x3F, 0xB1, 0x02, 0x85, 0xF2, 0x09, 0x49, 0x95, 0x0F, 0xB1, 0x02, 0x06,
            0x00, 0xFF, 0x85, 0x11, 0x09, 0x20, 0x15, 0x00, 0x26, 0xFF, 0x00, 0x75, 0x08, 0x95, 0x4D, 0x81, 0x02,
            0x09, 0x21, 0x91, 0x02, 0x85, 0x12, 0x09, 0x22, 0x95, 0x8D, 0x81, 0x02, 0x09, 0x23, 0x91, 0x02, 0x85,
            0x13, 0x09, 0x24, 0x95, 0xCD, 0x81, 0x02, 0x09, 0x25, 0x91, 0x02, 0x85, 0x14, 0x09, 0x26, 0x96, 0x0D,
            0x01, 0x81, 0x02, 0x09, 0x27, 0x91, 0x02, 0x85, 0x15, 0x09, 0x28, 0x96, 0x4D, 0x01, 0x81, 0x02, 0x09,
            0x29, 0x91, 0x02, 0x85, 0x16, 0x09, 0x2A, 0x96, 0x8D, 0x01, 0x81, 0x02, 0x09, 0x2B, 0x91, 0x02, 0x85,
            0x17, 0x09, 0x2C, 0x96, 0xCD, 0x01, 0x81, 0x02, 0x09, 0x2D, 0x91, 0x02, 0x85, 0x18, 0x09, 0x2E, 0x96,
            0x0D, 0x02, 0x81, 0x02, 0x09, 0x2F, 0x91, 0x02, 0x85, 0x19, 0x09, 0x30, 0x96, 0x22, 0x02, 0x81, 0x02,
            0x09, 0x31, 0x91, 0x02, 0x06, 0x80, 0xFF, 0x85, 0x82, 0x09, 0x22, 0x95, 0x3F, 0xB1, 0x02, 0x85, 0x83,
            0x09, 0x23, 0xB1, 0x02, 0x85, 0x84, 0x09, 0x24, 0xB1, 0x02, 0x85, 0x90, 0x09, 0x30, 0xB1, 0x02, 0x85,
            0x91, 0x09, 0x31, 0xB1, 0x02, 0x85, 0x92, 0x09, 0x32, 0xB1, 0x02, 0x85, 0x93, 0x09, 0x33, 0xB1, 0x02,
            0x85, 0x94, 0x09, 0x34, 0xB1, 0x02, 0x85, 0xA0, 0x09, 0x40, 0xB1, 0x02, 0x85, 0xA4, 0x09, 0x44, 0xB1,
            0x02, 0x85, 0xA7, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xA8, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xA9, 0x09, 0x45,
            0xB1, 0x02, 0x85, 0xAA, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xAB, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xAC, 0x09,
            0x45, 0xB1, 0x02, 0x85, 0xAD, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xB3, 0x09, 0x45, 0xB1, 0x02, 0x85, 0xB4,
            0x09, 0x46, 0xB1, 0x02, 0x85, 0xB5, 0x09, 0x47, 0xB1, 0x02, 0x85, 0xD0, 0x09, 0x40, 0xB1, 0x02, 0x85,
            0xD4, 0x09, 0x44, 0xB1, 0x02, 0xC0, 0x09, 0x02, 0x07, 0x35, 0x08, 0x35, 0x06, 0x09, 0x04, 0x09, 0x09,
            0x01, 0x00, 0x09, 0x02, 0x08, 0x28, 0x00, 0x09, 0x02, 0x09, 0x28, 0x01, 0x09, 0x02, 0x0A, 0x28, 0x01,
            0x09, 0x02, 0x0B, 0x09, 0x01, 0x00, 0x09, 0x02, 0x0C, 0x09, 0x1F, 0x40, 0x09, 0x02, 0x0D, 0x28, 0x00,
            0x09, 0x02, 0x0E, 0x28, 0x00
        };

        SdpPatcher.AlterHidDeviceToVendorDefined(buffer, out buffer);

        string hex = string.Join(", ", buffer.Select(b => $"0x{b:X2}"));


        Assert.Pass();
    }
}