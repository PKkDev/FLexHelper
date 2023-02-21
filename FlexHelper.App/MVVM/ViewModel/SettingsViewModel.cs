using System.Reflection;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.ApplicationModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Threading.Tasks;

namespace FlexHelper.App.MVVM.ViewModel;

public class SettingsViewModel : ObservableRecipient
{

    private string _versionDescription;
    public string VersionDescription
    {
        get => _versionDescription;
        set => SetProperty(ref _versionDescription, value);
    }

    private ElementTheme _elementTheme;
    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel()
    {
        _versionDescription = GetVersionDescription();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await SetThemeAsync(param);
                }
            });
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (false)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName"} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        await SetRequestedThemeAsync(theme);
    }

    public async Task SetRequestedThemeAsync(ElementTheme theme)
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = theme;

            //TitleBarHelper.UpdateTitleBar(theme);
        }

        await Task.CompletedTask;
    }
}
