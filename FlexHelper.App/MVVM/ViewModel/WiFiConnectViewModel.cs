using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexHelper.App.MVVM.Model;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.Security.Credentials;

namespace FlexHelper.App.MVVM.ViewModel;

public class WiFiConnectViewModel : ObservableRecipient
{
    public ObservableCollection<AdapterDisplayModel> WiFiAdapters { get; private set; }
    public ObservableCollection<WiFiNetworkDisplay> WiFiNetworks { get; private set; }

    private string _passwordWiFi;
    public string PasswordWiFi
    { get => _passwordWiFi; set => SetProperty(ref _passwordWiFi, value); }

    private WiFiNetworkDisplay _selectedWiFiNetwork;
    public WiFiNetworkDisplay SelectedWiFiNetwork
    { get => _selectedWiFiNetwork; set => SetProperty(ref _selectedWiFiNetwork, value); }

    private WiFiAdapter _selectedWiFiAdapter;
    public WiFiAdapter SelectedWiFiAdapter
    { get => _selectedWiFiAdapter; set => SetProperty(ref _selectedWiFiAdapter, value); }

    public ICommand SelectedWiFiAdapterChange { get; }
    public ICommand OnConnect { get; }

    public WiFiConnectViewModel()
    {
        WiFiAdapters = new();
        WiFiNetworks = new();

        SelectedWiFiAdapterChange = new RelayCommand<AdapterDisplayModel>(async (AdapterDisplayModel? param) =>
        {
            SelectedWiFiAdapter?.Disconnect();

            if (param != null)
            {
                SelectedWiFiAdapter = param.WiFiAdapter;
                await SelectedWiFiAdapter.ScanAsync();
                DisplayNetworkReport(SelectedWiFiAdapter);
            }
        });

        OnConnect = new RelayCommand(async () =>
        {
            if (SelectedWiFiAdapter == null || SelectedWiFiNetwork == null)
                return;

            // need password
            var notNeedPass = SelectedWiFiNetwork.AvailableNetwork.SecuritySettings.NetworkAuthenticationType == NetworkAuthenticationType.Open80211
            && SelectedWiFiNetwork.AvailableNetwork.SecuritySettings.NetworkEncryptionType == NetworkEncryptionType.None;

            WiFiReconnectionKind reconnectionKind = WiFiReconnectionKind.Manual;

            WiFiConnectionResult result;
            if (notNeedPass)
            {
                result = await SelectedWiFiAdapter.ConnectAsync(SelectedWiFiNetwork.AvailableNetwork, reconnectionKind);
            }
            else
            {
                // Only the password portion of the credential need to be supplied
                var credential = new PasswordCredential();

                // Make sure Credential.Password property is not set to an empty string. 
                // Otherwise, a System.ArgumentException will be thrown.
                // The default empty password string will still be passed to the ConnectAsync method,
                // which should return an "InvalidCredential" error
                if (!string.IsNullOrEmpty(PasswordWiFi))
                {
                    credential.Password = PasswordWiFi;
                }

                result = await SelectedWiFiAdapter.ConnectAsync(SelectedWiFiNetwork.AvailableNetwork, reconnectionKind, credential);
            }

        });

        LoadAdapters();
    }

    private async void DisplayNetworkReport(WiFiAdapter adapter)
    {
        WiFiNetworks.Clear();

        var connectedProfile = await adapter.NetworkAdapter.GetConnectedProfileAsync();
        foreach (var network in adapter.NetworkReport.AvailableNetworks)
        {
            WiFiNetworkDisplay networkDisplay = new(network, adapter);
            networkDisplay.UpdateConnectivityLevel(connectedProfile);
            WiFiNetworks.Add(networkDisplay);
        }
    }

    private async void LoadAdapters()
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        var networkInterfacesIsAv = NetworkInterface.GetIsNetworkAvailable();

        var bluetoothQuery = BluetoothAdapter.GetDeviceSelector();
        var result1 = await DeviceInformation.FindAllAsync(bluetoothQuery);

        var wiFiQuery = WiFiAdapter.GetDeviceSelector();
        var result = await DeviceInformation.FindAllAsync(wiFiQuery);
        foreach (DeviceInformation deviceInfo in result)
        {
            WiFiAdapter adapter = await WiFiAdapter.FromIdAsync(deviceInfo.Id);
            var connectedProfile = await adapter.NetworkAdapter.GetConnectedProfileAsync();
            WiFiAdapters.Add(new AdapterDisplayModel(deviceInfo, adapter));
        }
    }

    public void OnWiFiNetworkCLick(WiFiNetworkDisplay wifiNet)
    {
        SelectedWiFiNetwork = wifiNet;
        PasswordWiFi = null;
    }
}
