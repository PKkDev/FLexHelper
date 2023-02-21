using FlexHelper.App.MVVM.ViewModel;
using Microsoft.UI.Xaml.Controls;

namespace FlexHelper.App.MVVM.View;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<SettingsViewModel>();
    }
}
