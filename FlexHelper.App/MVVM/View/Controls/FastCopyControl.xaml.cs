using FlexHelper.App.MVVM.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
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

        //public FastCopyControlModel ViewModel { get; set; }

        public FastCopyControl()
        {
            InitializeComponent();
            //DataContext = ViewModel = App.GetService<FastCopyControlModel>();
        }

        private void UserNameBox_Toggle_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if ((sender as ToggleButton).IsChecked == true)
                UserNameBox.PasswordRevealMode = PasswordRevealMode.Visible;
            else
                UserNameBox.PasswordRevealMode = PasswordRevealMode.Hidden;
        }
        private void UserNameBox_Copy_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DataPackage dataPackage = new();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(UserNameBox.Password);
            Clipboard.SetContent(dataPackage);
        }
    }
}
