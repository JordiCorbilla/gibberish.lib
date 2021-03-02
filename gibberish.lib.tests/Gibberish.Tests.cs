using System.Security.Cryptography;
using NUnit.Framework;

namespace gibberish.lib.tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("Hi", "geEVOcuLz1mORg54Mwn1dQ==")]
        public void TestEncoding(string text, string encoded)
        {
            Assert.AreEqual(encoded, Gibberish.Encode<RijndaelManaged>(text));
        }

        [TestCase("geEVOcuLz1mORg54Mwn1dQ==", "Hi")]
        public void TestDecoding(string text, string decoded)
        {
            Assert.AreEqual(decoded, Gibberish.Decode<RijndaelManaged>(text));
        }

    }
}