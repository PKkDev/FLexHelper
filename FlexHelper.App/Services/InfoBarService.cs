using Microsoft.UI.Xaml.Controls;

namespace FlexHelper.App.Services
{
    public class InfoBarService
    {
        private InfoBar? _infoBar { get; set; }

        public InfoBarService() { }

        public void Initialization(InfoBar infoBar)
        {
            _infoBar = infoBar;
        }

        public void Show(string message, InfoBarSeverity severity = InfoBarSeverity.Informational)
        {
            if (_infoBar != null)
            {
                _infoBar.Message = message;
                _infoBar.Severity = severity;
                _infoBar.IsOpen = true;
            }
        }
    }
}
