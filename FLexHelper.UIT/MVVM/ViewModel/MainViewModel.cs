using FLexHelper.UIT.Core;
using FLexHelper.UIT.MVVM.View;
using FLexHelper.UIT.Services;
using System.Windows;

namespace FLexHelper.UIT.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelyCommand MouseMoverViewCommand { get; set; }

        public RelyCommand RDPConnectViewCommand { get; set; }

        public RelyCommand PeopleGameViewCommand { get; set; }

        public RelyCommand CloseAppCommand { get; set; }

        private object _currentView = ViewTypes.MouseMoverView;
        public object CurrentView
        {
            get => _currentView;
            set => OnSetNewValue(ref _currentView, value);
        }

        // public object CurrentView = ViewTypes.MouseMoverView;

        public MainViewModel()
        {
            ConfiguratorService.GetConfigFromFile();

            CurrentView = ViewTypes.MouseMoverView;

            CloseAppCommand = new RelyCommand(param => Application.Current.Shutdown());

            MouseMoverViewCommand = new RelyCommand(param => CurrentView = ViewTypes.MouseMoverView);
            RDPConnectViewCommand = new RelyCommand(param => CurrentView = ViewTypes.RDPConnectView);
            PeopleGameViewCommand = new RelyCommand(param => CurrentView = ViewTypes.PeopleGameView);
        }

    }
}
