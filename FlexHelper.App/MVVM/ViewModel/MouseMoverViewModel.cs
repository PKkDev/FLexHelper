using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlexHelper.App.Workers;

namespace FlexHelper.App.MVVM.ViewModel;

public class MouseMoverViewModel : ObservableRecipient
{
    private int _distance;
    public int Distance { get => _distance; set => SetProperty(ref _distance, value); }

    private int _interval;
    public int Interval { get => _interval; set => SetProperty(ref _interval, value); }

    private int _coefFast;
    public int CoefFast { get => _coefFast; set => SetProperty(ref _coefFast, value); }

    private string _btnText;
    public string BtnText { get => _btnText; set => SetProperty(ref _btnText, value); }

    public ICommand CycleCommand { get; private set; }

    public MouseMoverWorker _mouseMoverWorker;
    public MouseMoverWorker MouseMoverWorker { get => _mouseMoverWorker; set => SetProperty(ref _mouseMoverWorker, value); }

    private bool _workIsStarted = false;

    private Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

    public MouseMoverViewModel()
    {
        Distance = 150;
        Interval = 10;
        CoefFast = 1;
        BtnText = "Start";

        CycleCommand = new RelayCommand(() => OnCycleCommand());
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
