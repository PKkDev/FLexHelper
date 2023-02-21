using CommunityToolkit.Mvvm.ComponentModel;
using FLexHelper.Worker;
using Microsoft.UI.Dispatching;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FlexHelper.App.Workers
{
    public class MouseMoverWorker : ObservableObject
    {
        private readonly int _distance;
        private readonly int _interval;
        private readonly int _coefFast;
        private readonly DispatcherQueue _dispatcherQueue;

        private int _timerCounter;
        public int TimerCounter
        {
            get => _timerCounter;
            set { _dispatcherQueue.TryEnqueue(() => { SetProperty(ref _timerCounter, value); }); }
        }

        private int _workCounter;
        public int WorkCounter
        {
            get => _workCounter;
            set { _dispatcherQueue.TryEnqueue(() => { SetProperty(ref _workCounter, value); }); }
        }

        private int _maxWorkCounter;
        public int MaxWorkCounter
        {
            get => _maxWorkCounter;
            set { _dispatcherQueue.TryEnqueue(() => { SetProperty(ref _maxWorkCounter, value); }); }
        }

        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _ct;

        public MouseMoverWorker(int distance, int interval, int coefFast, DispatcherQueue dispatcherQueue)
        {
            _distance = distance;
            _interval = interval;
            _coefFast = coefFast;
            _dispatcherQueue = dispatcherQueue;

            MaxWorkCounter = _distance * 4 + 1;

            TimerCounter = 0;
            WorkCounter = 0;

            _tokenSource = new CancellationTokenSource();
            _ct = _tokenSource.Token;
        }

        public void Start()
        {
            TimerCounter = 0;
            WorkCounter = 0;

            var task = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        MouseWork();
                        SetInterval();
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    TimerCounter = 0;
                    WorkCounter = 0;
                }
            }, _ct);

            //Task.WhenAll(task);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        private void SetInterval()
        {
            TimerCounter = 0;
            var temo = _interval;
            while (temo > 0)
            {
                CheckCancel();
                temo--;
                TimerCounter = _interval - temo;
                Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
            }
            TimerCounter = 0;
        }

        private void MouseWork()
        {
            WorkCounter = 0;

            var delta = 1;
            var coefFast = _coefFast;

            CursorPoint cursorPos = new CursorPoint();
            Win32.GetPhysicalCursorPos(ref cursorPos);

            var tempoXPoseRight = 0;
            while (tempoXPoseRight < _distance)
            {
                CheckCancel();
                Win32.GetPhysicalCursorPos(ref cursorPos);
                cursorPos.x += delta * coefFast;
                Win32.SetCursorPos(cursorPos.x, cursorPos.y);
                tempoXPoseRight += delta * coefFast;
                WorkCounter += delta * coefFast;
                Thread.Sleep(10);
            }

            var tempoYPoseDown = 0;
            while (tempoYPoseDown < _distance)
            {
                CheckCancel();
                Win32.GetPhysicalCursorPos(ref cursorPos);
                cursorPos.y += delta * coefFast;
                Win32.SetCursorPos(cursorPos.x, cursorPos.y);
                tempoYPoseDown += delta * coefFast;
                WorkCounter += delta * coefFast;
                Thread.Sleep(10);
            }

            var tempoXPoseLeft = 0;
            while (tempoXPoseLeft < _distance)
            {
                CheckCancel();
                Win32.GetPhysicalCursorPos(ref cursorPos);
                cursorPos.x -= delta * coefFast;
                Win32.SetCursorPos(cursorPos.x, cursorPos.y);
                tempoXPoseLeft += delta * coefFast;
                WorkCounter += delta * coefFast;
                Thread.Sleep(10);
            }

            var tempoYPoseUp = 0;
            while (tempoYPoseUp < _distance)
            {
                CheckCancel();
                Win32.GetPhysicalCursorPos(ref cursorPos);
                cursorPos.y -= delta * coefFast;
                Win32.SetCursorPos(cursorPos.x, cursorPos.y);
                tempoYPoseUp += delta * coefFast;
                WorkCounter += delta * coefFast;
                Thread.Sleep(10);
            }

            CheckCancel();

            MouseEvent.ClickLeftMouseButton(cursorPos);

            WorkCounter = 0;

            CheckCancel();
        }

        private void CheckCancel()
        {
            if (_ct.IsCancellationRequested)
                _ct.ThrowIfCancellationRequested();
        }
    }
}
