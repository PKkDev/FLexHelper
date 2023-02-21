using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FLexHelper.Worker
{
    public class TestWorkerAutoConnect : INotifyPropertyChanged
    {
        private int _xPos { get; set; }
        private int _yPos { get; set; }
        private int _topMove { get; set; }
        private int _interval { get; set; }

        public int TimerCounter { get; set; }
        public int TimerMax { get; set; }

        public ObservableCollection<string> Logs { get; set; }
        private SynchronizationContext _uiContext { get; set; }

        private CancellationTokenSource _tokenSource { get; set; }
        private CancellationToken _ct { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public TestWorkerAutoConnect(int xPos, int yPos, int topMove, int interval, SynchronizationContext uiContext)
        {
            _xPos = xPos;
            _yPos = yPos;
            _topMove = topMove;
            _interval = interval * 60;
            TimerMax = interval * 60;

            _uiContext = uiContext;

            Logs = new ObservableCollection<string>();

            _tokenSource = new CancellationTokenSource();
            _ct = _tokenSource.Token;
        }

        public async Task Start()
        {
            TimerCounter = 0;

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
                OnPropertyChanged("TimerCounter");
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
            CursorPoint cursorPos = new CursorPoint();
            Win32.GetPhysicalCursorPos(ref cursorPos);

            Win32.SetCursorPos(_xPos, _yPos);

            Thread.Sleep(1000);
            MouseEvent.ClickRightMouseButton(cursorPos);
            Thread.Sleep(1000);

            Win32.SetCursorPos(_xPos - 5, _yPos - _topMove);

            Thread.Sleep(1000);
            MouseEvent.ClickLeftMouseButton(cursorPos);

            Win32.SetCursorPos(cursorPos.x, cursorPos.y);
        }

        public void Stop()
        {
            TimerCounter = 0;

            OnPropertyChanged("TimerCounter");

            _tokenSource.Cancel();
            WriteLog($"Task cancellation requested");
        }

        private void WriteLog(string message)
        {
            if (Logs != null)
                _uiContext.Send(x => Logs.Add(message), null);
            Console.WriteLine(message);
        }

        private void CheckCancel()
        {
            if (_ct.IsCancellationRequested)
            {
                WriteLog($"Task cancelled");
                _ct.ThrowIfCancellationRequested();
            }
        }
    }
}
