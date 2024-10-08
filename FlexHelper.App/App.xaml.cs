﻿using System;
using Microsoft.UI.Xaml;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using FlexHelper.App.MVVM.ViewModel;
using FlexHelper.App.MVVM.View;
using Microsoft.UI.Xaml.Controls;
using FlexHelper.App.Services; 
using FlexHelper.App.MVVM.View.Controls;

namespace FlexHelper.App;

public partial class App : Application
{
    public static Window MainWindow { get; set; }

    public IHost Host { get; }

    private UIElement? _shell;

#if IS_NONE_PACKAGE
        public static bool IsNonePackage { get => true; }
#else
    public static bool IsNonePackage { get => false; }
#endif

    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");

        return service;
    }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();

            services.AddTransient<ShellViewModel>();
            services.AddTransient<ShellPage>();

            services.AddTransient<WiFiConnectViewModel>();
            services.AddTransient<WiFiConnectPage>();

            services.AddTransient<MouseMoverViewModel>();
            services.AddTransient<MouseMoverPage>();

            services.AddTransient<FastTextViewModel>();
            services.AddTransient<FastTextView>(); 

            services.AddTransient<FastCopyControl>();

            services.AddSingleton<InfoBarService>();
            services.AddSingleton<ConfigService>();
            services.AddSingleton<NavigationHelperService>();
        })
        .Build();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();

        _shell = App.GetService<ShellPage>();
        MainWindow.Content = _shell ?? new Frame();

        SetWindowSize(MainWindow);

        MainWindow.Activate();
    }

    private void SetWindowSize(Window mainWindow)
    {
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 550, Height = 500 });

        //OverlappedPresenter overlappedPresenter = appWindow.Presenter as OverlappedPresenter;
        //overlappedPresenter.IsResizable = false;
    }
}
