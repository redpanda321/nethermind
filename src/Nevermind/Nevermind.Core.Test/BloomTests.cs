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

using System.Collections;
using System.Linq;
using Nevermind.Core.Crypto;
using Nevermind.Core.Encoding;
using Nevermind.Core.Extensions;
using NUnit.Framework;

namespace Nevermind.Core.Test
{
    [TestFixture]
    public class BloomTests
    {
        [Test]
        public void Test()
        {
            Bloom bloom = new Bloom();
            bloom.Set(Keccak.OfAnEmptyString.Bytes);
            byte[] bytes = bloom.Bytes;
            BitArray bits = bytes.ToBigEndianBitArray2048();
            Bloom bloom2 = new Bloom(bits);
            Assert.AreEqual(bloom.ToString(), bloom2.ToString());
        }
    }
}