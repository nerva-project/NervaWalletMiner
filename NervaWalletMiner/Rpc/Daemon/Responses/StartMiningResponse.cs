﻿using NervaWalletMiner.Rpc.Common;

namespace NervaWalletMiner.Rpc.Daemon.Responses
{
    public class StartMiningResponse
    {
        public ServiceError Error { get; set; } = new();

        public string Status { get; set; } = string.Empty;
        public string Untrusted { get; set; } = string.Empty;
    }
}