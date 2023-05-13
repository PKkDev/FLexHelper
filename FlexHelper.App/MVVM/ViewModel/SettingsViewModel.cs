using System.Reflection;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.ApplicationModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using FlexHelper.App.MVVM.Model;
using System.Linq;

namespace FlexHelper.App.MVVM.ViewModel;

public class SettingsViewModel : ObservableRecipient
{
    public ObservableCollection<UITheme> Themes { get; set; }
    private UITheme _selectedTheme;
    public UITheme SelectedTheme
    {
        get { return _selectedTheme; }
        set
        {
            SetTheme(value);
            SetProperty(ref _selectedTheme, value);
        }
    }

    public SettingsViewModel()
    {
        Themes = new()
        {
            new UITheme("Default", ElementTheme.Default),
            new UITheme("Dark", ElementTheme.Dark),
            new UITheme("Light", ElementTheme.Light),
        };

        if (App.MainWindow.Content is FrameworkElement rootElement2)
        {
            var theme = rootElement2.RequestedTheme;
            var search = Themes.FirstOrDefault(x => x.Theme == theme);
            if (search != null)
                SelectedTheme = search;
        }
    }

    public void SetTheme(UITheme theme)
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            if (rootElement.RequestedTheme == theme.Theme)
                return;

            rootElement.RequestedTheme = theme.Theme;
        }
    }
}
