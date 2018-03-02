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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Ethereum.Test.Base;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Core.Potocol;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Ethereum.KeyAddress.Test
{
    public class KeyAddressTests
    {
        private IEthereumSigner _signer;
        
        [OneTimeSetUp]
        public void SetUp()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            _signer = new EthereumSigner(Olympic.Instance, ChainId.MainNet);
        }

        private static IEnumerable<KeyAddressTest> LoadTests()
        {
            return TestLoader.LoadFromFile<KeyAddressTestJson[], KeyAddressTest>(
                "keyaddrtest.json",
                c => c.Select(p => new KeyAddressTest(
                    p.Seed,
                    p.Key,
                    p.Addr,
                    BigInteger.Parse(p.Signature.R),
                    BigInteger.Parse(p.Signature.S),
                    byte.Parse(p.Signature.V))));
        }

        [TestCase("0x135a7de83802408321b74c322f8558db1679ac20", "xyz",    "0x30755ed65396facf86c53e6217c52b4daebe72aa4941d89635409de4c9c7f9466d4e9aaec7977f05e923889b33c0d0dd27d7226b6e6f56ce737465c5cfd04be41b")]
        [TestCase("0x36d85Dc3683156e63Bf880A9fAb7788CF8143a27", "Christopher Pearce", "0x34ff4b97a0ec8f735f781f250dcd3070a72ddb640072dd39553407d0320db79939e3b080ecaa2e9f248214c6f0811fb4b4ba05b7bcff254c053e47d8513e82091b")]
        public void Test(string addressHex, string message, string sigHex)
        {
            Keccak messageHash = Keccak.Compute(message);
            Signature sig = new Signature(sigHex);
            Address recovered = _signer.RecoverAddress(sig, messageHash);
            Address address = new Address(addressHex);

            // TODO: check - at the moment they are failing when running in the test mode but not in Debug
            bool areEqaul = address.Equals(recovered);
            Assert.True(areEqaul);
            Assert.AreEqual(address, recovered);
        }

        [Ignore("either tests incorrect or I need to understand how R and S were represented")]
        [TestCaseSource(nameof(LoadTests))]
        public void Test(KeyAddressTest test)
        {
            PrivateKey privateKey = new PrivateKey(test.Key);
            Address actualAddress = privateKey.Address;
            Signature actualSig = _signer.Sign(privateKey, Keccak.OfAnEmptyString);
            string actualSigHex = actualSig.ToString();

            Signature expectedSig = new Signature(test.R, test.S, test.V);
            string expectedSigHex = expectedSig.ToString();
            Address expectedAddress = new Address(test.Address);

            Assert.AreEqual(expectedAddress, actualAddress);

            Address recoveredActualAddress = _signer.RecoverAddress(actualSig, Keccak.OfAnEmptyString);
            Assert.AreEqual(actualAddress, recoveredActualAddress);

            // it does not work
            Assert.AreEqual(expectedSigHex, actualSigHex);

            Address recovered = _signer.RecoverAddress(expectedSig, Keccak.OfAnEmptyString);
            Assert.AreEqual(expectedAddress, recovered);
        }

        private class SigOfEmptyString
        {
            public string V { get; set; }
            public string R { get; set; }
            public string S { get; set; }
        }

        private class KeyAddressTestJson
        {
            [JsonProperty("sig_of_emptystring")]
            public SigOfEmptyString Signature { get; set; }

            public string Seed { get; set; }
            public string Key { get; set; }
            public string Addr { get; set; }
        }

        public class KeyAddressTest
        {
            public KeyAddressTest(string seed, string key, string address, BigInteger r, BigInteger s, byte v)
            {
                Seed = seed;
                Key = key;
                Address = address;
                V = v;
                R = r;
                S = s;
            }

            public string Seed { get; }
            public string Key { get; }
            public string Address { get; }
            public byte V { get; }

            public BigInteger R { get; }
            public BigInteger S { get; }

            public override string ToString()
            {
                return $"{Seed}, exp: {R}, {S}, {V}";
            }
        }
    }
}