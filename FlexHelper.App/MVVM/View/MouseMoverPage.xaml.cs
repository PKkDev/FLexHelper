using FlexHelper.App.MVVM.ViewModel;
using Microsoft.UI.Xaml.Controls;

namespace FlexHelper.App.MVVM.View;

public sealed partial class MouseMoverPage : Page
{
    public MouseMoverViewModel ViewModel { get; }

    public MouseMoverPage()
    {
        InitializeComponent();
        DataContext = ViewModel = App.GetService<MouseMoverViewModel>();
    }
}
