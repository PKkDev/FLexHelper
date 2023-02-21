using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexHelper.App.MVVM.Model;
using Windows.Devices.Bluetooth;
using Windows.Devices.WiFi;

namespace FlexHelper.App.MVVM.ViewModel;

public class WiFiConnectViewModel : ObservableRecipient
{
    public ObservableCollection<WiFiAdapter> WiFiAdapters
    {
        get; private set;
    }
    public ObservableCollection<WiFiNetworkDisplay> WiFiNetworks
    {
        get; private set;
    }

    private WiFiAdapter _selectedWiFiAdapter;
    public WiFiAdapter SelectedWiFiAdapter
    {
        get => _selectedWiFiAdapter;
        set => SetProperty(ref _selectedWiFiAdapter, value);
    }

    public ICommand SelectedWiFiAdapterChange
    {
        get;
    }

    public WiFiConnectViewModel()
    {
        WiFiAdapters = new();
        WiFiNetworks = new();

        SelectedWiFiAdapterChange = new RelayCommand<WiFiAdapter>(async (WiFiAdapter? param) =>
        {
            if (SelectedWiFiAdapter != null)
            {
                SelectedWiFiAdapter.Disconnect();
            }

            if (param != null)
            {
                SelectedWiFiAdapter = param;
                await SelectedWiFiAdapter.ScanAsync();
                DisplayNetworkReport(SelectedWiFiAdapter);
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
            var networkDisplay = new WiFiNetworkDisplay(network, adapter);
            networkDisplay.UpdateConnectivityLevel(connectedProfile);
            WiFiNetworks.Add(networkDisplay);
        }
    }

    private async void LoadAdapters()
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        var networkInterfacesIsAv = NetworkInterface.GetIsNetworkAvailable();

        var bluetoothQuery = BluetoothAdapter.GetDeviceSelector();
        var result1 = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(bluetoothQuery);

        var wiFiQuery = WiFiAdapter.GetDeviceSelector();
        var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(wiFiQuery);
        foreach (var device in result)
        {
            WiFiAdapter adapter = await WiFiAdapter.FromIdAsync(device.Id);
            var connectedProfile = await adapter.NetworkAdapter.GetConnectedProfileAsync();
            WiFiAdapters.Add(adapter);
        }
    }
}
