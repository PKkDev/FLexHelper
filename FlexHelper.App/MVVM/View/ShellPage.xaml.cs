using FlexHelper.App.MVVM.ViewModel;
using FlexHelper.App.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FlexHelper.App.MVVM.View;

public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel { get; }

    public ShellPage()
    {
        InitializeComponent();
        DataContext = ViewModel = App.GetService<ShellViewModel>();

        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);

        ViewModel.NavigationHelperService.Initialize(NavView, ContentFrame);
        NavView.SelectedItem = NavView.MenuItems[0];
        ViewModel.NavigationHelperService.Navigate("MouseMover");

        var service = App.GetService<InfoBarService>();
        service.Initialization(PageInfoBar);
    }

    private void NavView_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }
}
