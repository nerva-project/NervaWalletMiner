﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using NervaOneWalletMiner.Helpers;
using NervaOneWalletMiner.Objects.Constants;
using NervaOneWalletMiner.Rpc;
using NervaOneWalletMiner.ViewModels;
using NervaOneWalletMiner.Views;
using System;

namespace NervaOneWalletMiner;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Want those before UI loads
        GlobalMethods.LoadConfig();

        SetUpDefaults();

        Logger.SetUpLogger();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += OnExit;
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
        Logger.LogDebug("App.IC", "Initialization completed");
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        Logger.LogDebug("App.AE", "Exiting...");

        Shutdown();
    }

    public static void Shutdown()
    {
        // Prevent the daemon restarting automatically before telling it to stop
        if (GlobalData.AppSettings.Daemon[GlobalData.AppSettings.ActiveCoin].StopOnExit)
        {
            //TODO: Call daemon method to exit
            //Daemon.StopDaemon();
            DaemonProcess.ForceClose();
        }

        WalletProcess.ForceClose();

        Logger.LogInfo("App.SD", "PROGRAM TERMINATED");

        Environment.Exit(0);
    }

    public static void SetUpDefaults()
    {
        try
        {
            // Set theme
            if(GlobalData.AppSettings.Theme == Theme.Default)
            {
                Application.Current!.RequestedThemeVariant = ThemeVariant.Default;
            }
            else if(GlobalData.AppSettings.Theme == Theme.Light)
            {
                Application.Current!.RequestedThemeVariant = ThemeVariant.Light;
            }
            else
            {
                Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
            }
            

            if (GlobalData.AppSettings.Daemon[GlobalData.AppSettings.ActiveCoin].MiningThreads == 0)
            {
                // By default use 50% of threads
                GlobalData.AppSettings.Daemon[GlobalData.AppSettings.ActiveCoin].MiningThreads = GlobalData.CpuThreadCount > 1 ? Convert.ToInt32(Math.Floor(GlobalData.CpuThreadCount / 2.00)) : 1;
            }

            GlobalMethods.SetCoin(GlobalData.AppSettings.ActiveCoin);

            // TODO: Might need to set up other defaults
        }
        catch (Exception ex)
        {
            Logger.LogException("App.SUD", ex);
        }
    }
}