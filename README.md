# gibberish.lib

Simple encoding/decoding library for .net 5 using the RFC 2898 - PKCS #5: Password-Based Cryptography Specification v2.0 <http://www.faqs.org/rfcs/rfc2898.html>

## Usage

```c#
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
```
