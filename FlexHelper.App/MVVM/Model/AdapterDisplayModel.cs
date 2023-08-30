using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;

namespace FlexHelper.App.MVVM.Model
{
    public class AdapterDisplayModel
    {
        public DeviceInformation DeviceInfo { get; set; }

        public WiFiAdapter WiFiAdapter { get; set; }

        public AdapterDisplayModel(DeviceInformation deviceInfo, WiFiAdapter wiFiAdapter)
        {
            DeviceInfo = deviceInfo;
            WiFiAdapter = wiFiAdapter;
        }
    }
}
