using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexHelper.App.MVVM.Model;
using FlexHelper.App.Services;
using FlexHelper.App.Workers;

namespace FlexHelper.App.MVVM.ViewModel;

public class MouseMoverViewModel : ObservableRecipient
{
    private int _distance;
    public int Distance
    {
        get => _distance;
        set
        {
            Config.MouseMoverSettings.Distance = value;
            SetProperty(ref _distance, value);
        }
    }

    private int _interval;
    public int Interval
    {
        get => _interval; 
        set
        {
            Config.MouseMoverSettings.Interval = value;
            SetProperty(ref _interval, value);
        }
    }

    private int _coefFast;
    public int CoefFast
    {
        get => _coefFast; 
        set
        {
            Config.MouseMoverSettings.CoefFast = value;
            SetProperty(ref _coefFast, value);
        }
    }

    private string _btnText;
    public string BtnText { get => _btnText; set => SetProperty(ref _btnText, value); }

    public ICommand CycleCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }

    public MouseMoverWorker _mouseMoverWorker;
    public MouseMoverWorker MouseMoverWorker { get => _mouseMoverWorker; set => SetProperty(ref _mouseMoverWorker, value); }

    private bool _workIsStarted = false;

    private Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

    private StorageSettings Config { get; set; }
    private readonly ConfigService _configService;

    public MouseMoverViewModel(ConfigService configService)
    {
        _configService = configService;
        Config = configService.GetConfig();

        Distance = Config.MouseMoverSettings.Distance;
        Interval = Config.MouseMoverSettings.Interval;
        CoefFast = Config.MouseMoverSettings.CoefFast;

        BtnText = "Start";

        CycleCommand = new RelayCommand(() => OnCycleCommand());

        SaveCommand = new RelayCommand(() => _configService.UpdateConfig(Config));
    }

    private void OnCycleCommand()
    {
        if (!_workIsStarted)
        {
            BtnText = "Stop";
            _workIsStarted = true;
            MouseMoverWorker = new(Distance, Interval, CoefFast, _dispatcherQueue);
            MouseMoverWorker.Start();
        }
        else
        {
            BtnText = "Start";
            _workIsStarted = false;
            MouseMoverWorker.Stop();
        }
    }
}
