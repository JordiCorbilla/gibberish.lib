# gibberish.lib

Simple encoding/decoding library for .net 5 using the RFC 2898 - PKCS #5: Password-Based Cryptography Specification v2.0 <http://www.faqs.org/rfcs/rfc2898.html>

## Usage

```c#
[TestCase("Hi", "Zj/WPvAsP4bY8Ga3TmHk7A==")]
public void TestEncoding(string text, string encoded)
{
    Assert.AreEqual(encoded, Gibberish.Encode<RijndaelManaged>(text));
}

[TestCase("Zj/WPvAsP4bY8Ga3TmHk7A==", "Hi")]
public void TestDecoding(string text, string decoded)
{
    Assert.AreEqual(decoded, Gibberish.Decode<RijndaelManaged>(text));
}
```
