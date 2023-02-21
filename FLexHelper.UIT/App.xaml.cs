using FLexHelper.Worker;
using System;
using System.Text;
using System.Threading;
using System.Windows;

namespace MouseMover.UIT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string UniqueEventName = "{30624aca-5659-4084-b3e1-820e55e52d6}";
        private const string UniqueMutexName = "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8}";
        private EventWaitHandle eventWaitHandle;
        private Mutex mutex;

        [MTAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            bool isOwned;
            mutex = new Mutex(true, UniqueMutexName, out isOwned);
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);

            //TestWorkerUSB.Start();

            GC.KeepAlive(mutex);

            if (isOwned)
            {
                base.OnStartup(e);

                // Spawn a thread which will be waiting for our event
                var thread = new Thread(
                    () =>
                    {
                        while (this.eventWaitHandle.WaitOne())
                        {
                            Current.Dispatcher.BeginInvoke(
                                (Action)(() =>
                                {
                                    Application.Current.MainWindow.Visibility = Visibility.Visible;
                                    Application.Current.MainWindow.Focus();
                                }));
                        }
                    });

                // It is important mark it as background otherwise it will prevent app from exiting.
                thread.IsBackground = true;

                thread.Start();

                //tb = (TaskbarIcon)FindResource("MyNotifyIcon");
                //tb.DataContext = new NotifyIconViewModel();
                //tb.Visibility = Visibility.Visible;

                //ToastNotificationManagerCompat.OnActivated += toastArgs =>
                //{
                //    // Obtain the arguments from the notification
                //    ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                //    // Obtain any user input (text boxes, menu selections) from the notification
                //    ValueSet userInput = toastArgs.UserInput;

                //    // Need to dispatch to UI thread if performing UI operations
                //    Application.Current.Dispatcher.Invoke(delegate
                //    {
                //        Application.Current.MainWindow.Visibility = Visibility.Visible;
                //    });
                //};
                mutex.ReleaseMutex();
            }
            else
            {
                eventWaitHandle.Set();
                App.Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
