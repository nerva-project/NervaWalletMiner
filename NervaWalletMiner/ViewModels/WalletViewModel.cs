﻿using Avalonia.Media.Imaging;
using NervaWalletMiner.Helpers;
using NervaWalletMiner.Objects.Constants;
using NervaWalletMiner.Objects.DataGrid;
using ReactiveUI;
using System.Collections.Generic;

namespace NervaWalletMiner.ViewModels
{
    internal class WalletViewModel : ViewModelBase
    {
        // TODO: Figure out how to do this in one place instead of on each view
        private Bitmap _CoinIcon = GlobalData.AppSettings.Misc[GlobalData.AppSettings.ActiveCoin].Logo;
        public Bitmap CoinIcon
        {
            get => _CoinIcon;
            set => this.RaiseAndSetIfChanged(ref _CoinIcon, value);
        }

        private string _OpenCloseWallet = StatusWallet.OpenWallet;
        public string OpenCloseWallet
        {
            get => _OpenCloseWallet;
            set => this.RaiseAndSetIfChanged(ref _OpenCloseWallet, value);
        }

        private string _TotalXnv = "";
        public string TotalXnv
        {
            get => _TotalXnv;
            set => this.RaiseAndSetIfChanged(ref _TotalXnv, value);
        }

        private string _UnlockedXnv = "";
        public string UnlockedXnv
        {
            get => _UnlockedXnv;
            set => this.RaiseAndSetIfChanged(ref _UnlockedXnv, value);
        }

        private List<Account> _WalletAddresses = new();
        public List<Account> WalletAddresses
        {
            get => _WalletAddresses;
            set => this.RaiseAndSetIfChanged(ref _WalletAddresses, value);
        }       
    }
}