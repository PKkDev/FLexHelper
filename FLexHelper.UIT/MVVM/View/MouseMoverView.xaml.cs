using FLexHelper.UIT.MVVM.ViewModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace FLexHelper.UIT.MVVM.View
{

    public partial class MouseMoverView : UserControl
    {
        private MouseMoverViewModel viewModel { get; set; }

        public MouseMoverView()
        {
            InitializeComponent();

            DataContext = viewModel = new MouseMoverViewModel();
        }

        private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex reg = new Regex("[^0-9]");
            e.Handled = reg.IsMatch(e.Text);
        }
    }
}
