using FlexHelper.App.MVVM.ViewModel;
using Microsoft.UI.Xaml.Controls;

namespace FlexHelper.App.MVVM.View;

public sealed partial class WiFiConnectPage : Page
{
    public WiFiConnectViewModel ViewModel
    {
        get;
    }

    public WiFiConnectPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<WiFiConnectViewModel>();
        DataContext = ViewModel;
    }
}
