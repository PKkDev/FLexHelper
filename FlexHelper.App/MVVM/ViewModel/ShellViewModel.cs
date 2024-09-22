using CommunityToolkit.Mvvm.ComponentModel;
using FlexHelper.App.Services;

namespace FlexHelper.App.MVVM.ViewModel;

public class ShellViewModel : ObservableRecipient
{
    public NavigationHelperService NavigationHelperService { get; init; }

    public ShellViewModel(NavigationHelperService navigationHelperService)
    {
        NavigationHelperService = navigationHelperService;
    }
}
