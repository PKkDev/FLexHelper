using System;
using System.Collections.Generic;
using System.Linq;
using FlexHelper.App.MVVM.ViewModel;
using FlexHelper.App.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace FlexHelper.App.MVVM.View;

public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel
    {
        get;
    }

    private readonly List<(string Tag, Type Page)> _pages = new()
    {
        ("MouseMover", typeof(MouseMoverPage)),
        ("WiFiConnect", typeof(WiFiConnectPage)),
        ("Settings", typeof(SettingsPage))
    };

    public ShellPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();

        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);

        var service = App.GetService<InfoBarService>();
        service.Initialization(PageInfoBar);

        Navigate("MouseMover");
    }

    private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {

    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked == true)
        {
            Navigate("Settings");
        }
        else if (args.InvokedItemContainer != null)
        {
            var navItemTag = args.InvokedItemContainer.Tag.ToString();
            Navigate(navItemTag);
        }
    }

    private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {

    }

    private void Navigate(string navItemTag)
    {
        Type _page = null;
        if (navItemTag == "Settings")
        {
            _page = typeof(SettingsPage);
        }
        else
        {
            var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
            _page = item.Page;
        }
        // Get the page type before navigation so you can prevent duplicate
        // entries in the backstack.
        var preNavPageType = ContentFrame.CurrentSourcePageType;

        // Only navigate if the selected page isn't currently loaded.
        if (!(_page is null) && !Type.Equals(preNavPageType, _page))
        {
            // ContentFrame.Navigate(_page, null);
            ContentFrame.Navigate(_page, null, new DrillInNavigationTransitionInfo());
        }
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
