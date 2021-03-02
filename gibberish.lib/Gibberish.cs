//  Copyright (c) 2021, Jordi Corbilla
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//
//  - Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//  - Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//  - Neither the name of this library nor the names of its contributors may be
//    used to endorse or promote products derived from this software without
//    specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
//  LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
//  SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace gibberish.lib
{
    public static class Gibberish
    {
        private const int PasswordIterations = 10000;
        private const int KeySize = 256;
        private static readonly byte[] DefaultVector = { 64, 69, 46, 30, 19, 49, 68, 57, 108, 54, 72, 56, 105, 53, 71, 52 };
        private static readonly byte[] DefaultSalt = { 115, 64, 49, 116, 67, 114, 121, 113, 112, 111 };
        private static readonly byte[] DefaultSecret = { 64, 98, 117, 121, 114, 136, 121, 98, 117, 121, 114, 136, 121, 98, 117, 122, 114, 136, 133, 98, 13, 121, 114, 136, 126 };

        public static string Encode<T>(string message, string secret = "", string salt = "")
            where T : SymmetricAlgorithm, new()
        {
            if (string.IsNullOrEmpty(message) || message == "")
                return "";
            var passPhrase = secret == "" ? DefaultSecret.ToUtf8() : secret;
            var saltValue = salt == "" ? DefaultSalt.ToUtf8() : salt;

            var messageBytes = message.ToAsciiBytes();

            var password = new Rfc2898DeriveBytes(passPhrase, 
                saltValue.ToAsciiBytes(), 
                PasswordIterations, 
                HashAlgorithmName.SHA512);

            var keyBytes = password.GetBytes(KeySize / 8);

            SymmetricAlgorithm symmetricKey = new T {Mode = CipherMode.CBC};

            var symmetricEncryptor = symmetricKey.CreateEncryptor(keyBytes, 
                DefaultVector.ToUtf8AsciiBytes());

            string cipherText;

            using (var memoryStream = new MemoryStream())
            {
                using var cryptoStream = new CryptoStream(memoryStream, 
                    symmetricEncryptor, 
                    CryptoStreamMode.Write);
                cryptoStream.Write(messageBytes, 0, messageBytes.Length);
                cryptoStream.FlushFinalBlock();
                cipherText = memoryStream.ToBase64();
            }

            return cipherText;
        }

        public static string Decode<T>(string gibberish, string secret = "", string salt = "")
            where T : SymmetricAlgorithm, new()
        {
            if (string.IsNullOrEmpty(gibberish) || gibberish == "")
                return "";

            var saltValue = salt == "" ? DefaultSalt.ToUtf8() : salt;
            var cipherTextBytes = gibberish.FromBase64();
            var password = new Rfc2898DeriveBytes(
                secret == "" ? DefaultSecret.ToUtf8() : secret,
                saltValue.ToAsciiBytes(), 
                PasswordIterations, 
                HashAlgorithmName.SHA512);

            var keyBytes = password.GetBytes(KeySize / 8);

            SymmetricAlgorithm symmetricKey = new T {Mode = CipherMode.CBC};
            var symmetricDecryptor = symmetricKey.CreateDecryptor(
                keyBytes,
                DefaultVector.ToUtf8AsciiBytes());

            string readable;
            using (var memoryStream = new MemoryStream(cipherTextBytes))
            {
                using var cryptoStream = new CryptoStream(memoryStream, symmetricDecryptor, CryptoStreamMode.Read);
                var plainTextBytes = new byte[cipherTextBytes.Length];
                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                readable = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }

            return readable;
        }
    }
}