using FLexHelper.UIT.MVVM.ViewModel;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace FLexHelper.UIT.MVVM.View
{
    /// <summary>
    /// Interaction logic for RDPConnectView.xaml
    /// </summary>
    public partial class RDPConnectView : UserControl
    {
        private RDPConnectViewModel viewModel { get; set; }

        public RDPConnectView()
        {
            InitializeComponent();

            DataContext = viewModel = new RDPConnectViewModel();
        }

        private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex reg = new Regex("[^0-9]");
            e.Handled = reg.IsMatch(e.Text);
        }
    }
}
