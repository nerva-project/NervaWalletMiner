using Avalonia.Controls;
using Avalonia.Interactivity;
using NervaOneWalletMiner.Helpers;
using NervaOneWalletMiner.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace NervaOneWalletMiner.ViewsDialogs
{
    public partial class OpenWalletView : Window
    {
        Window GetWindow() => TopLevel.GetTopLevel(this) as Window ?? throw new NullReferenceException("Invalid Owner");

        public OpenWalletView()
        {
            try
            {
                InitializeComponent();
                Icon = GlobalMethods.GetWindowIcon();

                if(!GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].IsPassRequiredToOpenWallet)
                {
                    tbxPassword.IsEnabled = false;
                    tbxPassword.Watermark = "Password not required to open wallet";
                    btnShowHidePassword.IsEnabled = false;
                }

                var walletName = this.Get<ComboBox>("cbxWalletName");

                walletName.ItemsSource = GetWalletFileNames();
                walletName.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logger.LogException("OWD.CONS", ex);
            }
        }

        public List<string> GetWalletFileNames()
        {
            List<string> wallets = [];
            
            try
            {
                DirectoryInfo walletsDir = new DirectoryInfo(GlobalMethods.GetWalletDir());
                if (GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].WalletExtension.Equals("directory"))
                {
                    DirectoryInfo[] dirs = walletsDir.GetDirectories();
                    if(dirs.Length > 0)
                    {
                        foreach(DirectoryInfo dir in dirs)
                        {
                            wallets.Add(dir.Name);
                        }
                    }
                }
                else
                {
                    FileInfo[] files = walletsDir.GetFiles("*" + GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].WalletExtension, SearchOption.TopDirectoryOnly);

                    if (files.Length > 0)
                    {
                        foreach (FileInfo file in files)
                        {
                            wallets.Add(Path.GetFileNameWithoutExtension(file.FullName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("OWD.GWFN", ex);
            }

            return wallets;
        }

        public void OkButton_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                if (GlobalData.CoinSettings[GlobalData.AppSettings.ActiveCoin].IsPassRequiredToOpenWallet && string.IsNullOrEmpty(tbxPassword.Text))
                {
                    MessageBoxView window = new("Open Wallet", "Password cannot be empty", true);
                    window.ShowDialog(GetWindow());
                }
                else if(string.IsNullOrEmpty(cbxWalletName.SelectedValue!.ToString()))
                {
                    MessageBoxView window = new("Open Wallet", "Wallet Name cannot be empty", true);
                    window.ShowDialog(GetWindow());
                }
                else
                {
                    DialogResult result = new()
                    {
                        IsOk = true,
                        WalletName = cbxWalletName.SelectedValue.ToString()!,
                        WalletPassword = tbxPassword.Text!
                    };

                    Close(result);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("OWD.OKBC", ex);
            }
        }

        public void CancelButton_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                DialogResult result = new()
                {
                    IsCancel = true
                };

                Close(result);
            }
            catch (Exception ex)
            {
                Logger.LogException("OWD.CLBC", ex);
            }
        }

        public void ShowHidePasswordButton_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                var tbxPassword = this.Get<TextBox>("tbxPassword");
                var btnShowHidePassword = this.Get<Button>("btnShowHidePassword");

                if (tbxPassword.RevealPassword)
                {
                    // Reveal was true, so hide
                    tbxPassword.RevealPassword = false;
                    btnShowHidePassword.Content = "Show";
                }
                else
                {
                    // Reveal was false, so show
                    tbxPassword.RevealPassword = true;
                    btnShowHidePassword.Content = "Hide";
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("OWD.SHPC", ex);
            }
        }
    }
}