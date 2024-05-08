﻿using NervaWalletMiner.Rpc.Common;
using NervaWalletMiner.Rpc.Daemon.Requests;
using NervaWalletMiner.Rpc.Daemon.Responses;
using System.Threading.Tasks;

namespace NervaWalletMiner.Rpc.Daemon
{
    public interface IDaemonService
    {
        Task<StartMiningResponse> StartMining(RpcSettings rpc, StartMiningRequest requestObj);

        Task<StopMiningResponse> StopMining(RpcSettings rpc, StopMiningRequest requestObj);
    }
}