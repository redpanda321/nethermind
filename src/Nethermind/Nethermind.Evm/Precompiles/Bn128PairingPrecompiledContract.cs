/*
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
using System.Numerics;
using Nethermind.Core.Crypto.ZkSnarks;
using Nethermind.Core.Extensions;

namespace Nethermind.Evm.Precompiles
{
    /// <summary>
    ///     Code adapted from ethereumJ (https://github.com/ethereum/ethereumj)
    /// </summary>
    public class Bn128PairingPrecompiledContract : IPrecompiledContract
    {
        private const int PairSize = 192;

        public static IPrecompiledContract Instance = new Bn128PairingPrecompiledContract();

        private Bn128PairingPrecompiledContract()
        {
        }

        public BigInteger Address => 8;

        public long BaseGasCost()
        {
            return 100000L;
        }

        public long DataGasCost(byte[] inputData)
        {
            if (inputData == null)
            {
                return 0L;
            }

            return 80000 * (inputData.Length / PairSize) + 100000;
        }

        public byte[] Run(byte[] inputData)
        {
            if (inputData == null)
            {
                inputData = Bytes.Empty;
            }

            // fail if input len is not a multiple of PAIR_SIZE
            if (inputData.Length % PairSize > 0)
            {
                throw new ArgumentException(); // TODO: check
            }

            PairingCheck check = PairingCheck.Create();

            // iterating over all pairs
            for (int offset = 0; offset < inputData.Length; offset += PairSize)
            {
                (Bn128Fp, Bn128Fp2) pair = DecodePair(inputData, offset);

                // fail if decoding has failed
                if (pair.Item1 == null || pair.Item2 == null)
                {
                    throw new ArgumentException();
                }

                check.AddPair(pair.Item1, pair.Item2);
            }

            check.Run();
            BigInteger result = check.Result();

            return result.ToBigEndianByteArray(32);
        }

        private (Bn128Fp, Bn128Fp2) DecodePair(byte[] input, int offset)
        {
            byte[] x = input.Slice(offset + 0, 32);
            byte[] y = input.Slice(offset + 32, 32);

            Bn128Fp p1 = Bn128Fp.CreateInG1(x, y);

            // fail if point is invalid
            if (p1 == null)
            {
                return (null, null);
            }

            // (b, a)
            byte[] b = input.Slice(offset + 64);
            byte[] a = input.Slice(offset + 96);

            // (d, c)
            byte[] d = input.Slice(offset + 128);
            byte[] c = input.Slice(offset + 160);

            Bn128Fp2 p2 = Bn128Fp2.CreateInG2(a, b, c, d);

            // fail if point is invalid
            if (p2 == null)
            {
                return (null, null);
            }

            return (p1, p2);
        }
    }
}