using Nefarius.Utilities.Bluetooth.SDP;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var d = BthPort.Devices.ToList();

            foreach (var device in d)
            {
                if (SdpPatcher.AlterHidDeviceToVenderDefined(device.CachedServices.ToArray(), out var patched))
                {
                    var t = 01;
                }
            }

            Assert.Pass();
        }
    }
}