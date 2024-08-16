using FLexHelper.Worker.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FLexHelper.Worker
{

    public class TestWorker : INotifyPropertyChanged
    {
        private readonly int _distance;
        private readonly int _interval;
        private readonly int _coefFast;

        public int TimerCounter { get; set; }
        public int WorkCounter { get; set; }
        public int MaxWorkCounter { get; set; }

        public ObservableCollection<string> Logs { get; set; }
        private SynchronizationContext _uiContext { get; set; }

        private CancellationTokenSource _tokenSource { get; set; }
        private CancellationToken _ct { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public TestWorker(int distance, int interval, int coefFast, SynchronizationContext uiContext)
        {
            _distance = distance;
            _interval = interval;
            _coefFast = coefFast;

            MaxWorkCounter = _distance * 4 + 1;

            TimerCounter = 0;
            WorkCounter = 0;

            _uiContext = uiContext;

            Logs = new ObservableCollection<string>();

            _tokenSource = new CancellationTokenSource();
            _ct = _tokenSource.Token;
        }

        public async Task Start()
        {
            TimerCounter = 0;
            WorkCounter = 0;

            var task = Task.Run(() => Cycle(), _ct);

            try
            {
                await Task.WhenAll(task);
            }
            catch (OperationCanceledException)
            {
                WriteLog($"{nameof(OperationCanceledException)} thrown");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                TimerCounter = 0;
                WorkCounter = 0;
                OnPropertyChanged("TimerCounter");
                OnPropertyChanged("WorkCounter");
            }
        }

        private void Cycle()
        {
            while (true)
            {
                MouseWork();
                SetInterval();
            }
        }

        public void Stop()
        {
            _tokenSource.Cancel();
            WriteLog($"Task cancellation requested");
        }

        private void CheckCancel()
        {
            if (_ct.IsCancellationRequested)
            {
                WriteLog($"Task cancelled");
                _ct.ThrowIfCancellationRequested();
            }
        }

        private void SetInterval()
        {
            TimerCounter = 0;
            OnPropertyChanged("TimerCounter");
            var temo = _interval;
            while (temo > 0)
            {
                CheckCancel();
                WriteLog($"Waiting {temo}");
                temo--;
                TimerCounter = _interval - temo;
                OnPropertyChanged("TimerCounter");
                Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
            }
            TimerCounter = 0;
            OnPropertyChanged("TimerCounter");
        }

        private void MouseWork()
        {
            try
            {
                WorkCounter = 0;
                WriteLog($"Start moving {DateTime.Now:HH:mm}");

                var delta = 1;
                var coefFast = _coefFast;

                CursorPoint cursorPos = new CursorPoint();
                Win32.GetPhysicalCursorPos(ref cursorPos);

                var tempoXPoseRight = 0;
                while (tempoXPoseRight < _distance)
                {
                    Win32.GetPhysicalCursorPos(ref cursorPos);
                    cursorPos.x += delta * coefFast;
                    Win32.SetCursorPos(cursorPos.x, cursorPos.y);
                    tempoXPoseRight += delta * coefFast;
                    WorkCounter += delta * coefFast;
                    OnPropertyChanged("WorkCounter");
                    Thread.Sleep(10);
                }

                CheckCancel();

                var tempoYPoseDown = 0;
                while (tempoYPoseDown < _distance)
                {
                    Win32.GetPhysicalCursorPos(ref cursorPos);
                    cursorPos.y += delta * coefFast;
                    Win32.SetCursorPos(cursorPos.x, cursorPos.y);
                    tempoYPoseDown += delta * coefFast;
                    WorkCounter += delta * coefFast;
                    OnPropertyChanged("WorkCounter");
                    Thread.Sleep(10);
                }

                CheckCancel();

                var tempoXPoseLeft = 0;
                while (tempoXPoseLeft < _distance)
                {
                    Win32.GetPhysicalCursorPos(ref cursorPos);
                    cursorPos.x -= delta * coefFast;
                    Win32.SetCursorPos(cursorPos.x, cursorPos.y);
                    tempoXPoseLeft += delta * coefFast;
                    WorkCounter += delta * coefFast;
                    OnPropertyChanged("WorkCounter");
                    Thread.Sleep(10);
                }

                CheckCancel();

                var tempoYPoseUp = 0;
                while (tempoYPoseUp < _distance)
                {
                    Win32.GetPhysicalCursorPos(ref cursorPos);
                    cursorPos.y -= delta * coefFast;
                    Win32.SetCursorPos(cursorPos.x, cursorPos.y);
                    tempoYPoseUp += delta * coefFast;
                    WorkCounter += delta * coefFast;
                    OnPropertyChanged("WorkCounter");
                    Thread.Sleep(10);
                }

                CheckCancel();

                MouseEvent.ClickLeftMouseButton(cursorPos);

                WorkCounter = 0;
                OnPropertyChanged("WorkCounter");

                CheckCancel();
            }
            catch (Exception e)
            {
                WriteLog($"{e.Message}");
                throw;
            }
            finally
            {
                WriteLog($"End moving");
            }
        }

        private void WriteLog(string message)
        {
            //if (Logs != null)
            //    _uiContext.Send(x => Logs.Add(message), null);
            //Console.WriteLine(message);
        }
    }
}
