﻿using NervaOneWalletMiner.Rpc.Common;

namespace NervaOneWalletMiner.Rpc.Wallet.Responses
{
    public class UnlockWithPassResponse
    {
        public ServiceError Error { get; set; } = new();
    }
}