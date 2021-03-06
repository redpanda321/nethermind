﻿/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Linq;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.Evm.Tracing
{
    public class ParityLikeBlockTracer : BlockTracerBase<ParityLikeTxTrace, ParityLikeTxTracer>
    {
        private Block _block;
        
        private readonly ParityTraceTypes _types;

        public ParityLikeBlockTracer(Keccak txHash, ParityTraceTypes types)
            :base(txHash)
        {
            _types = types;
        }
        
        public ParityLikeBlockTracer(ParityTraceTypes types)
        {
            _types = types;
        }

        protected override ParityLikeTxTracer OnStart(Keccak txHash)
        {
            return new ParityLikeTxTracer(_block, txHash == null ? null : _block.Transactions.Single(t => t.Hash == txHash), _types);
        }

        protected override ParityLikeTxTrace OnEnd(ParityLikeTxTracer txTracer)
        {
            return txTracer.BuildResult();
        }

        public override bool IsTracingRewards => true;

        public override void ReportReward(Address author, string rewardType, UInt256 rewardValue)
        {
            if ((_types & ParityTraceTypes.Trace) != 0)
            {
                ParityLikeTxTrace rewardTrace = TxTraces.Last();
                rewardTrace.Action = new ParityTraceAction();
                rewardTrace.Action.RewardType = rewardType;
                rewardTrace.Action.Value = rewardValue;
                rewardTrace.Action.Author = author;
                rewardTrace.Action.CallType = "reward";
                rewardTrace.Action.TraceAddress = new int[] { };
            }
        }

        public override void StartNewBlockTrace(Block block)
        {
            _block = block;
        }
    }
}