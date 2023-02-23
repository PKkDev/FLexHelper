using FlexHelper.App.MVVM.Model;
using FlexHelper.App.MVVM.ViewModel;
using Microsoft.UI.Xaml.Controls;

namespace FlexHelper.App.MVVM.View;

public sealed partial class WiFiConnectPage : Page
{
    public WiFiConnectViewModel ViewModel { get; }

    public WiFiConnectPage()
    {
        InitializeComponent();
        DataContext = ViewModel = App.GetService<WiFiConnectViewModel>();
    }

    private void WiFiListItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is WiFiNetworkDisplay wifiNet)
            ViewModel.OnWiFiNetworkCLick(wifiNet);
    }
}
