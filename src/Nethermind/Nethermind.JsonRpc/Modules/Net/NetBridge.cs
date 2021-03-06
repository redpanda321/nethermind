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

using System;
using Nethermind.Blockchain;
using Nethermind.Network;

namespace Nethermind.JsonRpc.Modules.Net
{
    public class NetBridge : INetBridge
    {
        private readonly ISynchronizationManager _syncManager;
        private readonly IPeerManager _peerManager;
        
        public NetBridge(ISynchronizationManager syncManager, IPeerManager peerManager)
        {
            _syncManager = syncManager ?? throw new ArgumentNullException(nameof(syncManager));
            _peerManager = peerManager ?? throw new ArgumentNullException(nameof(syncManager));
        }

        public int NetworkId => _syncManager.ChainId;
        public int PeerCount => _syncManager.GetPeerCount();
        
        public bool LogPeerConnectionDetails()
        {
            if (_peerManager == null)
            {
                return false;
            }
            _peerManager.LogSessionStats(true);
            return true;
        }
    }
}