using Avalonia.Controls;
using Avalonia.Interactivity;
using NervaOneWalletMiner.Objects.DataGrid;
using NervaOneWalletMiner.Objects;
using System;
using NervaOneWalletMiner.Helpers;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Threading;
using NervaOneWalletMiner.Rpc.Wallet.Requests;
using NervaOneWalletMiner.Rpc.Wallet.Responses;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

namespace NervaOneWalletMiner.ViewsDialogs
{
    public partial class AddressInfoView : Window
    {
        // <Label + AddressShort, AddressFull>
        Dictionary<string, string> _accounts = [];

        public AddressInfoView()
        {
            InitializeComponent();
        }

        public AddressInfoView(int accountIndex)
        {
            InitializeComponent();

            foreach (Account account in GlobalData.WalletStats.Subaddresses.Values)
            {
                string accountValue = string.IsNullOrEmpty(account.Label) ? "No label" + " (" + account.AddressShort + ")" : account.Label + " (" + account.AddressShort + ")";
                if (!_accounts.ContainsKey(accountValue))
                {
                    _accounts.Add(accountValue, account.AddressFull);
                }
            }

            var cbxAccount = this.Get<ComboBox>("cbxAccount");
            cbxAccount.ItemsSource = _accounts.Keys;
            cbxAccount.SelectedIndex = accountIndex;
        }

        private void AccountSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var cbxAccount = this.Get<ComboBox>("cbxAccount");
                var tbxWalletAddress = this.Get<TextBox>("tbxWalletAddress");
                if(_accounts.ContainsKey(cbxAccount.SelectedValue?.ToString()!))
                {
                    tbxWalletAddress.Text = _accounts[cbxAccount.SelectedValue?.ToString()!];
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("AIWal.ASC", ex);
            }                       
        }

        public void MakeIntegratedAddress_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                MakeIntegratedAddress(tbxWalletAddress.Text!);
            }
            catch (Exception ex)
            {
                Logger.LogException("AIWal.MIAC", ex);
            }
        }

        private async void MakeIntegratedAddress(string walletAddress)
        {
            try
            {
                // TODO: Current GUI does not pass payment_id. Do we need it?
                MakeIntegratedAddressRequest request = new()
                {
                    StandardAddress = walletAddress,
                };

                Logger.LogError("AIWal.MIA", "Making integrated address for: " + GlobalMethods.GetShorterString(walletAddress, 12));
                MakeIntegratedAddressResponse response = await GlobalData.WalletService.MakeIntegratedAddress(GlobalData.AppSettings.Wallet[GlobalData.AppSettings.ActiveCoin].Rpc, request);

                if (response.Error.IsError)
                {
                    Logger.LogError("AIWal.MIA", "Failed to make integrated address. Message: " + response.Error.Message + " | Code: " + response.Error.Code);
                    await Dispatcher.UIThread.Invoke(async () =>
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Make Integrated Address", "Error making integrated address\r\n" + response.Error.Message, ButtonEnum.Ok);
                        _ = await box.ShowAsync();
                    });
                }
                else
                {
                    Logger.LogError("AIWal.MIA", "Integrated address created successfully.");
                    tbxIntegratedAddress.Text = response.IntegratedAddress;
                    tbxPaymentId.Text = response.PaymentId;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("AIWal.MIA", ex);
            }
        }

        public void CloseButton_Clicked(object sender, RoutedEventArgs args)
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
                Logger.LogException("AIWal.CBC", ex);
            }
        }

        public void CopyWalletToClipboard_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                GlobalMethods.CopyToClipboard(this, tbxWalletAddress.Text!);            
            }
            catch (Exception ex)
            {
                Logger.LogException("NWal.CWC", ex);
            }
        }

        public void CopyIntegratedAddressToClipboard_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                GlobalMethods.CopyToClipboard(this, tbxIntegratedAddress.Text!);
            }
            catch (Exception ex)
            {
                Logger.LogException("NWal.CIAC", ex);
            }
        }

        public void CopyPaymentIdToClipboard_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                GlobalMethods.CopyToClipboard(this, tbxPaymentId.Text!);
            }
            catch (Exception ex)
            {
                Logger.LogException("NWal.CPIC", ex);
            }
        }
    }
}