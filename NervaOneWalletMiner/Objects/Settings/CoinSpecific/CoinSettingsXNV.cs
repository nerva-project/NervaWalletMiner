﻿using NervaOneWalletMiner.Helpers;
using NervaOneWalletMiner.Rpc.Common;

namespace NervaOneWalletMiner.Objects.Settings.CoinSpecific
{
    public class CoinSettingsXNV : ICoinSettings
    {
        #region Private Default Variables
        private int _DaemonPort = 17566;
        private string _DisplayUnits = "XNV";
        private int _LogLevelDaemon = 1;
        private int _LogLevelWallet = 1;
        private bool _IsCpuMiningPossible = true;

        private string _CliWin64Url = "https://github.com/nerva-project/nerva/releases/download/v0.1.8.0/nerva-v0.1.8.0_windows_minimal.zip";
        private string _CliWin32Url = "https://github.com/nerva-project/nerva/releases/download/v0.1.8.0/nerva-v0.1.8.0_windows_minimal.zip";
        private string _CliLin64Url = "https://github.com/nerva-project/nerva/releases/download/v0.1.8.0/nerva-v0.1.8.0_linux_minimal.zip";
        private string _CliLin32Url = "https://github.com/nerva-project/nerva/releases/download/v0.1.8.0/nerva-v0.1.8.0_linux_minimal.zip";
        private string _CliLinArmUrl = "https://github.com/nerva-project/nerva/releases/download/v0.1.8.0/nerva-v0.1.8.0_linux_minimal.zip";
        private string _CliMacIntelUrl = "https://github.com/nerva-project/nerva/releases/download/v0.1.8.0/nerva-v0.1.8.0_osx_minimal.zip";
        private string _CliMacArmUrl = "https://github.com/nerva-project/nerva/releases/download/v0.1.8.0/nerva-v0.1.8.0_osx_minimal.zip";

        private string _DataDirWin = "C:/ProgramData/nerva";
        private string _DataDirLin = "~/.nerva";
        private string _DataDirMac = "~/.nerva";

        private string _QuickSyncUrl = "https://nerva.one/quicksync/quicksync.raw";
        #endregion // Private Default Variables


        #region Interface Variables
        public int DaemonPort { get => _DaemonPort; set => _DaemonPort = value; }
        public string DisplayUnits { get => _DisplayUnits; set => _DisplayUnits = value; }
        public int LogLevelDaemon { get => _LogLevelDaemon; set => _LogLevelDaemon = value; }
        public int LogLevelWallet { get => _LogLevelWallet; set => _LogLevelWallet = value; }
        public bool IsCpuMiningPossible { get => _IsCpuMiningPossible; set => _IsCpuMiningPossible = value; }

        public string CliWin64Url { get => _CliWin64Url; set => _CliWin64Url = value; }
        public string CliWin32Url { get => _CliWin32Url; set => _CliWin32Url = value; }
        public string CliLin64Url { get => _CliLin64Url; set => _CliLin64Url = value; }
        public string CliLin32Url { get => _CliLin32Url; set => _CliLin32Url = value; }
        public string CliLinArmUrl { get => _CliLinArmUrl; set => _CliLinArmUrl = value; }
        public string CliMacIntelUrl { get => _CliMacIntelUrl; set => _CliMacIntelUrl = value; }
        public string CliMacArmUrl { get => _CliMacArmUrl; set => _CliMacArmUrl = value; }

        public string DataDirWin { get => _DataDirWin; set => _DataDirWin = value; }
        public string DataDirLin { get => _DataDirLin; set => _DataDirLin = value; }
        public string DataDirMac { get => _DataDirMac; set => _DataDirMac = value; }

        public string QuickSyncUrl { get => _QuickSyncUrl; set => _QuickSyncUrl = value; }
        #endregion // Interface Variables

        #region Interface Methods
        public string GenerateDaemonOptions(SettingsDaemon daemonSettings)
        {
            string daemonCommand = "--rpc-bind-port " + daemonSettings.Rpc.Port;
            daemonCommand += " --log-level " + daemonSettings.LogLevel;
            daemonCommand += " --log-file \"" + GlobalMethods.CycleLogFile(GlobalMethods.GetDaemonProcess()) + "\"";

            if (!string.IsNullOrEmpty(daemonSettings.DataDir))
            {
                daemonCommand += " --data-dir \"" + daemonSettings.DataDir + "\"";
            }

            if (daemonSettings.IsTestnet)
            {
                Logger.LogDebug("XNV.CGDO", "Connecting to testnet...");
                daemonCommand += " --testnet";
            }

            if (daemonSettings.AutoStartMining)
            {
                string miningAddress = daemonSettings.MiningAddress;
                Logger.LogDebug("XNV.CGDO", "Enabling startup mining @ " + miningAddress);
                daemonCommand += " --start-mining " + miningAddress + " --mining-threads " + daemonSettings.MiningThreads;
            }

            if (GlobalMethods.IsLinux())
            {
                daemonCommand += " --detach";
            }

            if (!string.IsNullOrEmpty(daemonSettings.AdditionalArguments))
            {
                daemonCommand += " " + daemonSettings.AdditionalArguments;
            }

            return daemonCommand;
        }

        public string GenerateWalletOptions(SettingsWallet walletSettings, RpcBase daemonRpc)
        {
            string appCommand = "--daemon-address " + daemonRpc.Host + ":" + daemonRpc.Port;
            appCommand += " --rpc-bind-port " + walletSettings.Rpc.Port;
            appCommand += " --disable-rpc-login";
            appCommand += " --wallet-dir \"" + GlobalData.WalletDir + "\"";
            appCommand += " --log-level " + walletSettings.LogLevel;
            appCommand += " --log-file \"" + GlobalMethods.CycleLogFile(GlobalMethods.GetRpcWalletProcess()) + "\"";

            if (GlobalData.AppSettings.Daemon[GlobalData.AppSettings.ActiveCoin].IsTestnet)
            {
                Logger.LogDebug("CGDO.CGWO", "Connecting to testnet...");
                appCommand += " --testnet";
            }

            // TODO: Uncomment to enable rpc user:pass.
            // string ip = d.IsPublic ? $" --rpc-bind-ip 0.0.0.0 --confirm-external-bind" : $" --rpc-bind-ip 127.0.0.1";
            // appCommand += $"{ip} --rpc-login {d.Login}:{d.Pass}";

            return appCommand;
        }
        #endregion // Interface Methods
    }
}