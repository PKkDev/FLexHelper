using FlexHelper.App.MVVM.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.DataTransfer;

namespace FlexHelper.App.MVVM.View.Controls
{
    public sealed partial class FastCopyControl : UserControl
    {
        public static readonly DependencyProperty FastTextModelProperty =
            DependencyProperty.Register(
                nameof(FastTextModel),
                typeof(FastTextModel),
                typeof(FastCopyControl),
                new PropertyMetadata(null));

        public FastTextModel FastTextModel
        {
            get => (FastTextModel)GetValue(FastTextModelProperty);
            set => SetValue(FastTextModelProperty, value);
        }

        public FastCopyControl()
        {
            InitializeComponent();
        }

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
                PassBox.PasswordRevealMode = PasswordRevealMode.Visible;
            else
                PassBox.PasswordRevealMode = PasswordRevealMode.Hidden;
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(PassBox.Password);
            Clipboard.SetContent(dataPackage);
        }
    }
}
