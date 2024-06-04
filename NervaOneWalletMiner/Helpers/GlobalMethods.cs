﻿using Avalonia.Media.Imaging;
using Avalonia.Platform;
using NervaOneWalletMiner.Objects.Constants;
using NervaOneWalletMiner.Objects.Settings;
using NervaOneWalletMiner.Objects.Settings.CoinSpecific;
using NervaOneWalletMiner.Rpc;
using NervaOneWalletMiner.Rpc.Daemon;
using NervaOneWalletMiner.Rpc.Common;
using NervaOneWalletMiner.Rpc.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Avalonia.Threading;
using NervaOneWalletMiner.Rpc.Daemon.Requests;
using NervaOneWalletMiner.Rpc.Daemon.Responses;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using NervaOneWalletMiner.Rpc.Wallet.Requests;
using NervaOneWalletMiner.Rpc.Wallet.Responses;
using NervaOneWalletMiner.Objects.DataGrid;
using Avalonia.Input;
using Avalonia.Controls;

namespace NervaOneWalletMiner.Helpers
{
    public static class GlobalMethods
    {
        public static readonly Bitmap _walletImage = new Bitmap(AssetLoader.Open(new Uri("avares://NervaOneWalletMiner/Assets/wallet.png")));

        #region Directories, Paths and Names
        public static string GetDataDir()
        {
            string dataDirectory;

            // Get data directory
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string? homeDir = Environment.GetEnvironmentVariable("HOME");
                if (homeDir != null)
                {
                    dataDirectory = Path.Combine(homeDir, GlobalData.AppName);
                }
                else
                {
                    throw new DirectoryNotFoundException("Non-Windows dir not found");
                }
            }
            else
            {
                string? appdataDir = Environment.GetEnvironmentVariable("APPDATA");
                if (appdataDir != null)
                {
                    dataDirectory = Path.Combine(appdataDir, GlobalData.AppName);
                }
                else
                {
                    throw new DirectoryNotFoundException("Windows dir not found");
                }
            }

            // Create data directory if it does not exist
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            if(!Directory.Exists(dataDirectory))
            {
                throw new Exception("Data directory not set up. Application cannot continue");
            }

            return dataDirectory;
        }

        public static string GetLogDir()
        {
            string logDirectory;

            if (Directory.Exists(GlobalData.DataDir))
            {
                // Create logs directory if it does not exist
                logDirectory = Path.Combine(GlobalData.DataDir, GlobalData.LogsDirName);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            else
            {
                throw new Exception("Data directory not set up. Application cannot continue");
            }

            return logDirectory;
        }

        public static string GetWalletDir()
        {
            string walletDirectory;

            if (Directory.Exists(GlobalData.DataDir))
            {
                // Create logs directory if it does not exist
                walletDirectory = Path.Combine(GlobalData.DataDir, GlobalData.MainCoinsDirName, GlobalData.CoinDirName, GlobalData.WalletDirName);
                if (!Directory.Exists(walletDirectory))
                {
                    Directory.CreateDirectory(walletDirectory);
                }
            }
            else
            {
                throw new Exception("Data directory not set up. Application cannot continue");
            }

            return walletDirectory;
        }

        public static string GetCliToolsDir()
        {
            string cliToolsDirectory;

            if (Directory.Exists(GlobalData.DataDir))
            {
                // Create logs directory if it does not exist
                cliToolsDirectory = Path.Combine(GlobalData.DataDir, GlobalData.MainCoinsDirName, GlobalData.CoinDirName, GlobalData.CliToolsDirName);
                if (!Directory.Exists(cliToolsDirectory))
                {
                    Directory.CreateDirectory(cliToolsDirectory);
                }
            }
            else
            {
                throw new Exception("Data directory not set up. Application cannot continue");
            }

            return cliToolsDirectory;
        }

        public static string GetDaemonProcess()
        {
            return Path.Combine(GlobalData.CliToolsDir, GlobalData.DaemonProcessName);
        }

        public static string GetRpcWalletProcess()
        {
            return Path.Combine(GlobalData.CliToolsDir, GlobalData.WalletProcessName);
        }

        public static string GetConfigFilePath()
        {           
            string dataDir = GetDataDir();
            return Path.Combine(dataDir, "app.config");
        }
        #endregion // Directories, Paths and Names

        #region Coin Specific Setup
        public static Dictionary<string, ICoinSettings> GetDefaultCoinSettings()
        {
            Dictionary<string, ICoinSettings> defaultSettings = [];

            defaultSettings.Add(Coin.XNV, new CoinSettingsXNV());
            defaultSettings.Add(Coin.XMR, new CoinSettingsXMR());

            return defaultSettings;
        }
        public static Dictionary<string, SettingsDaemon> GetDaemonSettings()
        {
            if (GlobalData.CoinSettings == null || GlobalData.CoinSettings.Count == 0)
            {
                GlobalData.CoinSettings = GetDefaultCoinSettings();
            }

            Dictionary<string, SettingsDaemon> daemonSettings = [];

            daemonSettings.Add(Coin.XNV, new SettingsDaemon() { 
                BlockSeconds = GlobalData.CoinSettings[Coin.XNV].BlockSeconds,
                LogLevel = GlobalData.CoinSettings[Coin.XNV].LogLevelDaemon,
                Rpc = new RpcBase() { 
                    Port = GlobalData.CoinSettings[Coin.XNV].DaemonPort
                }
            });

            daemonSettings.Add(Coin.XMR, new SettingsDaemon()
            {
                BlockSeconds = GlobalData.CoinSettings[Coin.XMR].BlockSeconds,
                LogLevel = GlobalData.CoinSettings[Coin.XMR].LogLevelDaemon,
                Rpc = new RpcBase()
                {
                    Port = GlobalData.CoinSettings[Coin.XMR].DaemonPort
                }
            });

            return daemonSettings;
        }

        public static Dictionary<string, SettingsWallet> GetWalletSettings()
        {
            if(GlobalData.CoinSettings == null || GlobalData.CoinSettings.Count == 0)
            {
                GlobalData.CoinSettings = GetDefaultCoinSettings();
            }

            Dictionary<string, SettingsWallet> walletSettings = [];

            walletSettings.Add(Coin.XNV, new SettingsWallet()
            {
                DisplayUnits = GlobalData.CoinSettings[Coin.XNV].DisplayUnits,
                LogLevel = GlobalData.CoinSettings[Coin.XNV].LogLevelDaemon,
                Rpc = new RpcBase()
                {
                    Port = (uint)GlobalData.RandomGenerator.Next(10000, 50000)
                }
            });

            walletSettings.Add(Coin.XMR, new SettingsWallet()
            {
                DisplayUnits = GlobalData.CoinSettings[Coin.XMR].DisplayUnits,
                LogLevel = GlobalData.CoinSettings[Coin.XMR].LogLevelDaemon,
                Rpc = new RpcBase()
                {
                    Port = (uint)GlobalData.RandomGenerator.Next(10000, 50000)
                }
            });

            return walletSettings;
        }

        public static void SetCoin(string newCoin)
        {
            // TODO: Need to do certain tings when switching coins. RpcConnection? Wallet?

            switch(newCoin)
            {
                case Coin.XMR:
                    Logger.LogDebug("GM.SC", "Setting up: " + Coin.XMR);
                    GlobalData.CoinDirName = Coin.XMR;
                    GlobalData.AppSettings.ActiveCoin = Coin.XMR;                   
                    GlobalData.CliToolsDir = GetCliToolsDir();
                    GlobalData.WalletDir = GetWalletDir();

                    GlobalData.WalletProcessName = GetWalletProcessName();
                    GlobalData.DaemonProcessName = GetDaemonProcessName();
                    GlobalData.Logo = GetLogo();

                    GlobalData.DaemonService = new DaemonServiceXMR();
                    GlobalData.WalletService = new WalletServiceXMR();

                    // TODO: Change this. App.config overwrites GetDaemonSettings with 0
                    if (GlobalData.AppSettings.Daemon[Coin.XMR].BlockSeconds != GlobalData.CoinSettings[Coin.XMR].BlockSeconds)
                    {
                        GlobalData.AppSettings.Daemon[Coin.XMR].BlockSeconds = GlobalData.CoinSettings[Coin.XMR].BlockSeconds;
                    }
                    if (GlobalData.AppSettings.Daemon[Coin.XMR].LogLevel != GlobalData.CoinSettings[Coin.XMR].LogLevelDaemon)
                    {
                        GlobalData.AppSettings.Daemon[Coin.XMR].LogLevel = GlobalData.CoinSettings[Coin.XMR].LogLevelDaemon;
                    }
                    if (GlobalData.AppSettings.Wallet[Coin.XMR].DisplayUnits != GlobalData.CoinSettings[Coin.XMR].DisplayUnits)
                    {
                        GlobalData.AppSettings.Wallet[Coin.XMR].DisplayUnits = GlobalData.CoinSettings[Coin.XMR].DisplayUnits;
                    }
                    if (GlobalData.AppSettings.Wallet[Coin.XMR].LogLevel != GlobalData.CoinSettings[Coin.XMR].LogLevelWallet)
                    {
                        GlobalData.AppSettings.Wallet[Coin.XMR].LogLevel = GlobalData.CoinSettings[Coin.XMR].LogLevelWallet;
                    }
                    break;

                default:
                    // XNV or anything else not supported
                    Logger.LogDebug("GM.SC", "Setting up: " + Coin.XNV);
                    GlobalData.CoinDirName = Coin.XNV;
                    GlobalData.AppSettings.ActiveCoin = Coin.XNV;
                    GlobalData.CliToolsDir = GetCliToolsDir();
                    GlobalData.WalletDir = GetWalletDir();

                    GlobalData.WalletProcessName = GetWalletProcessName();
                    GlobalData.DaemonProcessName = GetDaemonProcessName();
                    GlobalData.Logo = GetLogo();

                    GlobalData.DaemonService = new DaemonServiceXNV();
                    GlobalData.WalletService = new WalletServiceXNV();

                    // TODO: Change this. App.config overwrites GetDaemonSettings() with default 0
                    if (GlobalData.AppSettings.Daemon[Coin.XNV].BlockSeconds != GlobalData.CoinSettings[Coin.XNV].BlockSeconds)
                    {
                        GlobalData.AppSettings.Daemon[Coin.XNV].BlockSeconds = GlobalData.CoinSettings[Coin.XNV].BlockSeconds;
                    }
                    break;
            }

            Logger.LogDebug("GM.SC", "Coin set up: " + GlobalData.AppSettings.ActiveCoin);


            // Download CLI tools, if we do not have them already
            if (!DirectoryContainsCliTools(GlobalData.CliToolsDir))
            {
                string cliToolsLink = GetCliToolsDownloadLink();
                Logger.LogDebug("GM.SC", "CLI tools not found. Attempting to download from: " + cliToolsLink);
                if (!string.IsNullOrEmpty(cliToolsLink))
                {
                    SetUpCliTools(cliToolsLink, GlobalData.CliToolsDir);
                }
            }
        }

        public static string GetCliToolsDownloadLink()
        {
            string cliDownloadLink = string.Empty;

            try
            {
                Architecture arch = GetCpuArchitecture();

                if (IsWindows())
                {
                    switch (arch)
                    {
                        case Architecture.X64:
                            cliDownloadLink = GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].CliWin64Url;
                            break;
                        default:
                            cliDownloadLink = GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].CliWin32Url;
                            break;
                    }
                }
                else if (IsLinux())
                {
                    switch (arch)
                    {
                        case Architecture.X64:
                            cliDownloadLink = GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].CliLin64Url;
                            break;
                        case Architecture.Arm:
                        case Architecture.Arm64:
                            cliDownloadLink = GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].CliLinArmUrl;
                            break;
                        default:
                            cliDownloadLink = GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].CliLin32Url;
                            break;
                    }
                }
                else if (IsOsx())
                {
                    switch (arch)
                    {
                        case Architecture.Arm:
                        case Architecture.Arm64:
                            cliDownloadLink = GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].CliMacArmUrl;
                            break;
                        default:
                            cliDownloadLink = GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].CliMacIntelUrl;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.GCTDL", ex);
            }
            
            return cliDownloadLink;
        }

        public static async void SetUpCliTools(string downloadUrl, string cliToolsPath)
        {
            // Check if we already downloaded the CLI package
            string destFileWithPath = Path.Combine(cliToolsPath, Path.GetFileName(downloadUrl));

            if (File.Exists(destFileWithPath))
            {
                Logger.LogDebug("GM.SUCT", "Extracting existing CLI tools: " + destFileWithPath);

                ExtractFile(cliToolsPath, destFileWithPath);
            }
            else
            {
                Logger.LogDebug("GM.SUCT", "Downloading CLI tools. URL: " + downloadUrl);

                bool isSuccess = await DownloadFileToFolder(downloadUrl, cliToolsPath);

                if(isSuccess)
                {
                    Logger.LogDebug("GM.SUCT", "Extracting CLI tools after download: " + destFileWithPath);
                    ExtractFile(cliToolsPath, destFileWithPath);
                }
            }
        }

        public static async Task<bool> DownloadFileToFolder(string downloadUrl, string destinationDir)
        {
            bool isSuccess = false;

            try
            {
                if (!Directory.Exists(destinationDir))
                {
                    Directory.CreateDirectory(destinationDir);
                }

                string destFile = Path.Combine(destinationDir, Path.GetFileName(downloadUrl));

                Logger.LogDebug("GM.DFTF", "Downloading file: " + downloadUrl + " to: " + destFile);
                using (HttpClient client = new())
                {
                    using (var clientStream = await client.GetStreamAsync(downloadUrl))
                    {
                        using (var fileStream = new FileStream(destFile, FileMode.Create))
                        {
                            await clientStream.CopyToAsync(fileStream);
                            Logger.LogDebug("GM.DFTF", "Setting success for: " + downloadUrl);
                            isSuccess = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.DFTF", ex);
            }

            return isSuccess;
        }

        private static void ExtractFile(string destDir, string destFile)
        {
            try
            {
                Logger.LogDebug("GM.EF", "Closing Daemon and Wallet processes");
                while (DaemonProcess.IsRunning())
                {
                    DaemonProcess.ForceClose();
                    Thread.Sleep(1000);
                }

                while (WalletProcess.IsRunning())
                {
                    WalletProcess.ForceClose();
                    Thread.Sleep(1000);
                }

                Logger.LogDebug("GM.EF", "Extracting CLI tools");

                ZipArchive archive = ZipFile.Open(destFile, ZipArchiveMode.Read);
                foreach (var entry in archive.Entries)
                {
                    if (!string.IsNullOrEmpty(entry.Name))
                    {
                        Logger.LogDebug("GM.EF", "Extracting: " + entry.Name);
                        string extFile = Path.Combine(destDir, entry.Name);
                        entry.ExtractToFile(extFile, true);
#if UNIX
                        UnixNative.Chmod(extFile, 33261);
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.EF", ex);
            }
        }

        public static string GetDaemonProcessName()
        {
            string daemonProcess;

            switch (GlobalData.AppSettings.ActiveCoin)
            {
                case Coin.XMR:
                    daemonProcess = GlobalMethods.IsWindows()? "monerod.exe" : "monerod";
                    break;
                default:
                    // XNV or anything else not supported
                    daemonProcess = GlobalMethods.IsWindows()? "nervad.exe" : "nervad";
                    break;
            }

            return daemonProcess;
        }

        public static string GetWalletProcessName()
        {
            string walletProcess;

            switch (GlobalData.AppSettings.ActiveCoin)
            {
                case Coin.XMR:
                    walletProcess = GlobalMethods.IsWindows() ? "monero-wallet-rpc.exe" : "monero-wallet-rpc";
                    break;
                default:
                    // XNV or anything else not supported
                    walletProcess = GlobalMethods.IsWindows() ? "nerva-wallet-rpc.exe" : "nerva-wallet-rpc";
                    break;
            }

            return walletProcess;
        }

        public static Bitmap GetLogo()
        {
            Bitmap logo;

            switch (GlobalData.AppSettings.ActiveCoin)
            {
                case Coin.XMR:
                    logo = new Bitmap(AssetLoader.Open(new Uri("avares://NervaOneWalletMiner/Assets/xmr/logo.png")));
                    break;
                default:
                    // XNV or anything else not supported
                    logo = new Bitmap(AssetLoader.Open(new Uri("avares://NervaOneWalletMiner/Assets/xnv/logo.png")));
                    break;
            }

            return logo;
        }
        #endregion // Coin Specific Setup

        public static string GetShorterString(string? text, int shorterLength)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int charsOnEachSide = shorterLength / 2;
            return text.Length > shorterLength ? text.Substring(0, charsOnEachSide) + "..." + text.Substring(text.Length - charsOnEachSide, charsOnEachSide) : text;
        }

        public static DateTime UnixTimeStampToDateTime(ulong utcTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(utcTimeStamp);
        }

        public static ulong DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            return (ulong)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static void SaveConfig()
        {
            try
            {
                var contentsToWriteToFile = Newtonsoft.Json.JsonConvert.SerializeObject(GlobalData.AppSettings);
                using (TextWriter writer = new StreamWriter(GlobalData.ConfigFilePath))
                {
                    writer.Write(contentsToWriteToFile);
                }                    
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.SC", ex);
            }
        }

        public static void LoadConfig()
        {
            try
            {
                if(File.Exists(GlobalData.ConfigFilePath))
                {
                    using (TextReader reader = new StreamReader(GlobalData.ConfigFilePath))
                    {
                        var fileContents = reader.ReadToEnd();
                        ApplicationSettings settings = Newtonsoft.Json.JsonConvert.DeserializeObject<ApplicationSettings>(fileContents)!;
                        if (settings != null)
                        {
                            GlobalData.AppSettings = settings;
                        }
                    }
                }            
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.LC", ex);
            }
        }

        public static string GenerateRandomString(int length)
        {
            Random random = new Random();

            char[] charArray = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            string text = string.Empty;
            for (int i = 0; i < length; i++)
            {
                text += charArray[random.Next(0, charArray.Length)];
            }

            return text;
        }

        public static string CycleLogFile(string path)
        {
            string logFile = path + ".log";
            string oldLogFile = logFile + ".old";

            try
            {
                if (File.Exists(oldLogFile))
                {
                    File.Delete(oldLogFile);
                }

                if (File.Exists(logFile))
                {
                    File.Move(logFile, oldLogFile);
                }
            }
            catch (Exception)
            {
                Logger.LogError("GM.CLF", $"Cannot cycle log file. New log will be written to {logFile}");
                return logFile;
            }

            return logFile;
        }

        public static bool IsWindows()
        {
            GetCpuArchitecture();

            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);            
        }

        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static bool IsOsx()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public static Architecture GetCpuArchitecture()
        {
            return RuntimeInformation.OSArchitecture;
        }

        public static bool DirectoryContainsCliTools(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            bool hasDaemon = File.Exists(Path.Combine(path, GlobalData.DaemonProcessName));
            bool hasRpcWallet = File.Exists(Path.Combine(path, GlobalData.WalletProcessName));

            return (hasRpcWallet && hasDaemon);
        }

        public static List<string> GetSupportedLanguages()
        {
            List<string> languages = [];
            try
            {
                languages.Add(Language.English);
                languages.Add(Language.German);
                languages.Add(Language.Spanish);
                languages.Add(Language.French);
                languages.Add(Language.Italian);
                languages.Add(Language.Dutch);
                languages.Add(Language.Portuguese);
                languages.Add(Language.Russian);
                languages.Add(Language.Chinese_Simplified);
                languages.Add(Language.Esperanto);
                languages.Add(Language.Lojban);
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.GSL", ex);
            }

            return languages;
        }

        public static async void StartMiningAsync(int threads)
        {
            try
            {
                StartMiningRequest request = new()
                {
                    MiningAddress = GlobalData.AppSettings.Daemon[GlobalData.AppSettings.ActiveCoin].MiningAddress,
                    ThreadCount = threads
                };

                Logger.LogDebug("GM.StMA", "Calling StartMining. Address: " + GlobalMethods.GetShorterString(request.MiningAddress, 12) + " | Threads: " + request.ThreadCount);
                StartMiningResponse response = await GlobalData.DaemonService.StartMining(GlobalData.AppSettings.Daemon[GlobalData.AppSettings.ActiveCoin].Rpc, request);
                if (response.Error.IsError)
                {
                    await Dispatcher.UIThread.Invoke(async () =>
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Start Mining", "Error when starting mining\r\n\r\n" + response.Error.Message, ButtonEnum.Ok);
                        _ = await box.ShowAsync();
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.StMA", ex);
            }
        }

        public static async void StopMiningAsync()
        {
            try
            {
                StopMiningRequest request = new();

                Logger.LogDebug("GM.SpMA", "Calling StopMining.");
                StopMiningResponse response = await GlobalData.DaemonService.StopMining(GlobalData.AppSettings.Daemon[GlobalData.AppSettings.ActiveCoin].Rpc, request);
                if (response.Error.IsError)
                {
                    await Dispatcher.UIThread.Invoke(async () =>
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Stop Mining", "Error when stopping mining\r\n\r\n" + response.Error.Message, ButtonEnum.Ok);
                        _ = await box.ShowAsync();
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.SpMA", ex);
            }
        }

        public static async void SaveWallet()
        {
            try
            {
                SaveWalletResponse resStore = await GlobalData.WalletService.SaveWallet(GlobalData.AppSettings.Wallet[GlobalData.AppSettings.ActiveCoin].Rpc, new SaveWalletRequest());

                if (resStore.Error.IsError)
                {
                    Logger.LogError("GM.SW", "Error saving wallet: " + GlobalData.OpenedWalletName + ". Code: " + resStore.Error.Code + ", Message: " + resStore.Error.Message);
                }
                else
                {
                    Logger.LogDebug("GM.SW", "Wallet " + GlobalData.OpenedWalletName + " saved!");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.SW", ex);
            }
        }

        public static async void WalletUiUpdate()
        {
            try
            {
                // Get accounts for Wallets view
                GetAccountsResponse resGetAccounts = await GlobalData.WalletService.GetAccounts(GlobalData.AppSettings.Wallet[GlobalData.AppSettings.ActiveCoin].Rpc, new GetAccountsRequest());

                if (resGetAccounts.Error.IsError)
                {
                    Logger.LogError("GM.WUU", "GetAccounts Error Code: " + resGetAccounts.Error.Code + ", Message: " + resGetAccounts.Error.Message);
                }
                else
                {
                    GlobalData.WalletStats.TotalBalanceLocked = resGetAccounts.BalanceLocked;
                    GlobalData.WalletStats.TotalBalanceUnlocked = resGetAccounts.BalanceUnlocked;

                    GlobalData.WalletStats.Subaddresses = [];

                    // TODO: Set icon inside CallAsync method above?
                    foreach (Account account in resGetAccounts.SubAccounts)
                    {
                        account.WalletIcon = _walletImage;

                        GlobalData.WalletStats.Subaddresses.Add(account.Index, account);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.WUU", ex);
            }
        }

        public static string GenerateRandomHexString(int length, bool upperCase = false)
        {
            char[] array = ("0123456789" + (upperCase ? "ABCDEF" : "abcdef")).ToCharArray();
            string text = string.Empty;
            for (int i = 0; i < length; i++)
            {
                text += array[GlobalData.RandomGenerator.Next(0, array.Length)];
            }

            return text;
        }

        public static void CopyToClipboard(Avalonia.Visual visual, string text)
        {
            try
            {
                var clipboard = TopLevel.GetTopLevel(visual)?.Clipboard;
                var dataObject = new DataObject();
                dataObject.Set(DataFormats.Text, text);

                if (clipboard != null)
                {
                    clipboard.SetDataObjectAsync(dataObject);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("GM.CTC", ex);
            }
        }
    }
}