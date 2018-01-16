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

using HashLib;

namespace Nevermind.Core.Crypto
{
    public static class Ripemd
    {
        private static readonly IHash Hash = HashFactory.Crypto.CreateRIPEMD160();

        public static byte[] Compute(byte[] input)
        {
            return Hash.ComputeBytes(input).GetBytes();
        }

        public static string ComputeString(byte[] input)
        {
            return Hex.FromBytes(Compute(input), false);
        }

        public static byte[] Compute(string input)
        {
            return Compute(System.Text.Encoding.UTF8.GetBytes(input));
        }

        public static string ComputeString(string input)
        {
            return ComputeString(System.Text.Encoding.UTF8.GetBytes(input));
        }
    }
}