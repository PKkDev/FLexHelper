using Microsoft.UI.Xaml.Controls;
using FlexHelper.App.MVVM.ViewModel;

namespace FlexHelper.App.MVVM.View
{
    public sealed partial class FastTextView : Page
    {
        public FastTextViewModel ViewModel { get; }

        public FastTextView()
        {
            InitializeComponent();
            DataContext = ViewModel = App.GetService<FastTextViewModel>();
        }
    }
}
