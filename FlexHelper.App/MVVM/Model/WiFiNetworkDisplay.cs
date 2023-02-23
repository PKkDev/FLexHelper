using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;

namespace FlexHelper.App.MVVM.Model;

public class WiFiNetworkDisplay : ObservableObject
{
    private readonly WiFiAdapter _adapter;

    private WiFiAvailableNetwork _availableNetwork;
    public WiFiAvailableNetwork AvailableNetwork
    { get => _availableNetwork; private set => SetProperty(ref _availableNetwork, value); }

    private BitmapImage _wiFiImage;
    public BitmapImage WiFiImage
    { get => _wiFiImage; private set => SetProperty(ref _wiFiImage, value); }

    private string _connectivityLevel;
    public string ConnectivityLevel
    { get => _connectivityLevel; private set => SetProperty(ref _connectivityLevel, value); }

    public string Ssid => _availableNetwork.Ssid;

    public string Bssid => _availableNetwork.Bssid;

    public string ChannelCenterFrequency => string.Format("{0}kHz", _availableNetwork.ChannelCenterFrequencyInKilohertz);

    public string Rssi => string.Format("{0}dBm", _availableNetwork.NetworkRssiInDecibelMilliwatts);

    public string SecuritySettings => string.Format("Authentication: {0}; Encryption: {1}", _availableNetwork.SecuritySettings.NetworkAuthenticationType, _availableNetwork.SecuritySettings.NetworkEncryptionType);

    public WiFiNetworkDisplay(WiFiAvailableNetwork availableNetwork, WiFiAdapter adapter)
    {
        AvailableNetwork = availableNetwork;
        _adapter = adapter;
        UpdateWiFiImage();
    }

    private void UpdateWiFiImage()
    {
        var imageFileNamePrefix = "secure";
        if (AvailableNetwork.SecuritySettings.NetworkAuthenticationType == NetworkAuthenticationType.Open80211)
            imageFileNamePrefix = "open";

        var imageFileName = string.Format("ms-appx:/Assets/WiFiControls/{0}_{1}bar.png", imageFileNamePrefix, AvailableNetwork.SignalBars);

        WiFiImage = new BitmapImage(new Uri(imageFileName));
    }

    public void UpdateConnectivityLevel(ConnectionProfile connectedProfile)
    {
        var connectivityLevel = "Not Connected";
        string connectedSsid = null;

        //var connectedProfile = await _adapter.NetworkAdapter.GetConnectedProfileAsync();
        if (connectedProfile != null &&
            connectedProfile.IsWlanConnectionProfile &&
            connectedProfile.WlanConnectionProfileDetails != null)
        {
            connectedSsid = connectedProfile.WlanConnectionProfileDetails.GetConnectedSsid();
        }

        if (!string.IsNullOrEmpty(connectedSsid))
        {
            if (connectedSsid.Equals(AvailableNetwork.Ssid))
            {
                    //connectivityLevel = connectedProfile.GetNetworkConnectivityLevel().ToString();
            }
        }

        ConnectivityLevel = connectivityLevel;
    }
}
