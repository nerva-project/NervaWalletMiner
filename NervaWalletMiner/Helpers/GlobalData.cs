﻿using NervaWalletMiner.Objects.DataGrid;
using NervaWalletMiner.Objects.Settings;
using NervaWalletMiner.Objects.Stats;
using NervaWalletMiner.Rpc.Daemon;
using NervaWalletMiner.Rpc.Wallet;
using System;
using System.Collections.Generic;

namespace NervaWalletMiner.Helpers
{
    public static class GlobalData
    {
        public const string AppName = "NervaWalletMiner";
        public const string Version = "0.5.0";

        public const string CliToolsDirName = "cli";
        public const string WalletDirName = "wallets";
        public const string LogsDirName = "logs";
        public const string MainCoinsDirName = "coins";
        public static string CoinDirName = "xnv";

        public static Random RandomGenerator = new Random();

        // Those will be saved to and read from app.config
        public static ApplicationSettings AppSettings = new();

        public static readonly string DataDir = GlobalMethods.GetDataDir();
        public static readonly string CliToolsDir = GlobalMethods.GetCliToolsDir();
        public static readonly string WalletDir = GlobalMethods.GetWalletDir();
        public static readonly string LogDir = GlobalMethods.GetLogDir();
        public static readonly string ConfigFilePath = GlobalMethods.GetConfigFilePath();

        public static bool IsWalletOpen = false;
        public static bool IsWalletJustOpened = true;

        public static StatsDaemon NetworkStats = new();
        public static StatsWallet WalletStats = new();
        public static StatsTransfers TransfersStats = new();
        public static List<Connection> Connections = new List<Connection>();

        public static ulong NewestTransactionHeight = 0;

        public static int CpuThreadCount = Environment.ProcessorCount;

        // Coin specific
        public static IWalletService WalletService = new WalletServiceXNV();
        public static IDaemonService DaemonService = new DaemonServiceXNV();

        public static string WalletProcessName = GlobalMethods.GetWalletProcessName();
        public static string DaemonProcessName = GlobalMethods.GetDaemonProcessName();
    }
}