﻿namespace NervaWalletMiner.Helpers
{
    public static class GlobalData
    {
        public const string AppName = "NervaWalletMiner";
        public const string LogsDirName = "logs";
        public const string WalletDirName = "wallets";
        public const string Version = "0.5.0";

        public static readonly string DataDir = GlobalMethods.GetDataDir();
        public static readonly string LogDir = GlobalMethods.GetLogDir();
        public static readonly string WalletDir = GlobalMethods.GetWalletDir();
    }
}