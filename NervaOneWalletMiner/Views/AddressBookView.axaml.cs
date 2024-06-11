using Avalonia.Controls;
using Avalonia.Interactivity;
using NervaOneWalletMiner.Helpers;
using NervaOneWalletMiner.Objects.Constants;
using NervaOneWalletMiner.Objects.DataGrid;
using NervaOneWalletMiner.ViewModels;
using NervaOneWalletMiner.ViewsDialogs;
using System;

namespace NervaOneWalletMiner.Views
{
    public partial class AddressBookView : UserControl
    {
        Window GetWindow() => TopLevel.GetTopLevel(this) as Window ?? throw new NullReferenceException("Invalid Owner");

        public AddressBookView()
        {
            try
            {
                InitializeComponent();
                imgCoinIcon.Source = GlobalMethods.GetLogo();

                GlobalMethods.LoadAddressBook();

                if (GlobalData.AddressBook.List.Count == 0)
                {
                    // Add blank row so user can add new address
                    GlobalData.AddressBook.List.Add(new AddressInfo());
                }
                else
                {
                    AddressInfo addressWithHighestId = GetAddressWithHighestId();

                    if (!string.IsNullOrEmpty(addressWithHighestId.Address))
                    {
                        // Only add new row if there isn't one already based on Address
                        GlobalData.AddressBook.List.Add(new AddressInfo { Id = addressWithHighestId.Id + 1 });
                    }
                }

                dtgAddressBook.ItemsSource = GlobalData.AddressBook.List;
            }
            catch (Exception ex)
            {
                Logger.LogException("ADB.CONS", ex);
            }
        }

        private void AddressBook_RowEditEnded(object? sender, DataGridRowEditEndedEventArgs e)
        {
            try
            {
                // Just save. Don't need to complicate things
                GlobalMethods.SaveAddressBook();

                // Add blank row if new one was just added
                AddressInfo addressWithHighestId = GetAddressWithHighestId();
                if (!string.IsNullOrEmpty(addressWithHighestId.Address))
                {
                    GlobalData.AddressBook.List.Add(new AddressInfo { Id = addressWithHighestId.Id + 1 });
                }
            }
            catch (Exception ex)
            {
                Logger.LogException("ADB.ABRE", ex);
            }
        }

        private void Transfer_Clicked(object sender, RoutedEventArgs args)
        {
            try
            {
                AddressInfo selectedAddress = (AddressInfo)dtgAddressBook.SelectedItem;

                if(selectedAddress == null)
                {
                    MessageBoxView window = new("Transfer", "Please select Address first.", true);
                    window.ShowDialog(GetWindow());
                }
                else
                {
                    if(!GlobalData.IsWalletTransferRegistered)
                    {
                        MessageBoxView window = new("Transfer", "Please open wallet first.", true);
                        window.ShowDialog(GetWindow());
                    }
                    else
                    {
                        ((WalletViewModel)GlobalData.ViewModelPages[SplitViewPages.Wallet]).Transfer(GetWindow(), selectedAddress.Address, selectedAddress.PaymentId);
                    }                    
                }                
            }
            catch (Exception ex)
            {
                Logger.LogException("ADB.TRCL", ex);
            }
        }

        private AddressInfo GetAddressWithHighestId()
        {
            AddressInfo highestIdAddress = new();

            try
            {
                if (GlobalData.AddressBook.List.Count > 0)
                {
                    highestIdAddress = GlobalData.AddressBook.List[0];
                    foreach (AddressInfo address in GlobalData.AddressBook.List)
                    {
                        if (address.Id > highestIdAddress.Id)
                        {
                            highestIdAddress = address;
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                Logger.LogException("ADB.GAHI", ex);
            }

            return highestIdAddress;
        }
    }
}