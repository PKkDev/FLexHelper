using FLexHelper.UIT.Core;
using FLexHelper.UIT.MVVM.Model;
using FLexHelper.UIT.Services;
using FLexHelper.Worker;
using System.Threading;

namespace FLexHelper.UIT.MVVM.ViewModel
{
    public class MouseMoverViewModel : ObservableObject
    {
        private int _distance;
        public int Distance
        {
            get { return _distance; }
            set { OnSetNewValue(ref _distance, value); }
        }

        private int _interval;
        public int Interval
        {
            get { return _interval; }
            set { OnSetNewValue(ref _interval, value); }
        }

        private int _coefFast;
        public int CoefFast
        {
            get { return _coefFast; }
            set { OnSetNewValue(ref _coefFast, value); }
        }

        private string _btnText;
        public string BtnText
        {
            get { return _btnText; }
            set { OnSetNewValue(ref _btnText, value); }
        }

        public RelyCommand CycleCommand { get; set; }
        public RelyCommand SaveConfigCommand { get; set; }

        public TestWorker testWorker { get; set; }
        private bool _workIsStarted = false;
        private AppSettingsConfig config;

        public MouseMoverViewModel()
        {
            config = ConfiguratorService.GetConfig();

            Distance = config.MouseMoverSettings.Distance;
            Interval = config.MouseMoverSettings.Interval;
            CoefFast = config.MouseMoverSettings.CoefFast;
            BtnText = "Start";

            CycleCommand = new RelyCommand(param => this.OnCycleCommand());

            SaveConfigCommand = new RelyCommand(param =>
            {
                config.MouseMoverSettings.Distance = Distance;
                config.MouseMoverSettings.Interval = Interval;
                config.MouseMoverSettings.CoefFast = CoefFast;
                ConfiguratorService.UpdateConfig(config);
            });
        }

        private void OnCycleCommand()
        {
            if (!_workIsStarted)
            {
                BtnText = "Stop";
                _workIsStarted = true;
                var uiContext = SynchronizationContext.Current;
                testWorker = new TestWorker(Distance, Interval, CoefFast, uiContext);
                OnPropertyChanged("testWorker");
                testWorker.Start();
            }
            else
            {
                BtnText = "Start";
                _workIsStarted = false;
                testWorker.Stop();
            }
        }
    }
}
