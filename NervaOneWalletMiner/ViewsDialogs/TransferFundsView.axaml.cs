using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using NervaOneWalletMiner.Helpers;
using NervaOneWalletMiner.Objects;
using NervaOneWalletMiner.Objects.DataGrid;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NervaOneWalletMiner.ViewsDialogs
{
    public partial class TransferFundsView : Window
    {
        Window GetWindow() => TopLevel.GetTopLevel(this) as Window ?? throw new NullReferenceException("Invalid Owner");

        Dictionary<uint, string> _accounts = [];

        public TransferFundsView()
        {
            InitializeComponent();
        }

        public TransferFundsView(int accountIndex)
        {
            InitializeComponent();

            foreach(Account account in GlobalData.WalletStats.Subaddresses.Values)
            {
                if(!_accounts.ContainsKey(account.Index))
                {
                    _accounts.Add(account.Index, string.IsNullOrEmpty(account.Label) ? "No label" + " (" + account.AddressShort + ")" : account.Label + " (" + account.AddressShort + ")");
                }
            }

            var cbxSendFrom = this.Get<ComboBox>("cbxSendFrom");

            cbxSendFrom.ItemsSource = _accounts.Values;
            cbxSendFrom.SelectedIndex = accountIndex;            
        }

        public async void OkButtonClicked(object sender, RoutedEventArgs args)
        {
            try
            {
                var cbxSendFrom = this.Get<ComboBox>("cbxSendFrom");
                var tbxSendTo = this.Get<TextBox>("tbxSendTo");
                var tbxAmount = this.Get<TextBox>("tbxAmount");
                var tbxPaymentId = this.Get<TextBox>("tbxPaymentId");

                if (string.IsNullOrEmpty(cbxSendFrom.SelectedValue!.ToString()) || string.IsNullOrEmpty(tbxSendTo.Text) || string.IsNullOrEmpty(tbxAmount.Text))
                {
                    await Dispatcher.UIThread.Invoke(async () =>
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Transfer Funds", "From Address, To Address and Amount are required", ButtonEnum.Ok);
                        _ = await box.ShowAsync();
                    });
                }
                else
                {
                    uint fromAccountIndex = 0;
                    string fromAddress = cbxSendFrom.SelectedValue.ToString()!;

                    // TODO: Since addresses are shortened, you could potentally have 2 of the same ones
                    foreach (uint index in _accounts.Keys)
                    {
                        if (_accounts[index].Equals(fromAddress))
                        {
                            fromAccountIndex = index;
                        }
                    }

                    MessageBoxView window = new MessageBoxView("Confirm Transfer", "You're about to send " + tbxAmount.Text
                        + " " + GlobalData.AppSettings.Wallet[GlobalData.AppSettings.ActiveCoin].DisplayUnits
                        + ". Once transfer is started, it cannot be stopped. Do you want to continue?");
                    DialogResult confirmRes = await window.ShowDialog<DialogResult>(this);

                    if (confirmRes != null && confirmRes.IsOk)
                    {
                        DialogResult result = new()
                        {
                            IsOk = true,
                            SendFromAddress = fromAddress,
                            SendFromAddressIndex = fromAccountIndex,
                            SendToAddress = tbxSendTo.Text,
                            SendAmount = Convert.ToDecimal(tbxAmount.Text),
                            SendPaymentId = tbxPaymentId.Text!
                        };

                        Close(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("TFWal.OBC", ex);
            }
        }

        public void CancelButtonClicked(object sender, RoutedEventArgs args)
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
                Logger.LogException("TFWal.CBC", ex);
            }
        }

        public void GeneratePaymentId_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                tbxPaymentId.Text = GlobalMethods.GenerateRandomHexString(64);
            }
            catch (Exception ex)
            {
                Logger.LogException("TFWal.GPIC", ex);
            }
        }
    }
}
