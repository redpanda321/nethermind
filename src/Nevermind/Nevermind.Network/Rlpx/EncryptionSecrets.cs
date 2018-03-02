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

using Org.BouncyCastle.Crypto.Digests;

namespace Nevermind.Network.Rlpx
{
    public class EncryptionSecrets
    {
        public KeccakDigest EgressMac { get; set; }
        public KeccakDigest IngressMac { get; set; }
        public byte[] AesSecret { get; set; }
        public byte[] MacSecret { get; set; }
        public byte[] Token { get; set; }
    }
}