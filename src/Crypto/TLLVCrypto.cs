using System;
using System.IO;
using System.Security.Cryptography;

using FoolishTech.FairPlay.Entities;
using FoolishTech.Support.Throws;


namespace FoolishTech.FairPlay.Crypto
{
    internal static class TLLVCrypto 
    {        
        public static byte[] UnscrambledPayload(this SPCMessage message, RSAParameters parameters)
        {
            ArgumentThrow.IfNull(message, $"Invalid {typeof(SPCMessage).Name} to unscramble. Message can not be null.", nameof(message));
            ArgumentThrow.IfNull(parameters, $"Invalid {typeof(SPCMessage).Name} to unscramble. {typeof(RSAParameters).Name} can not be null.", nameof(parameters));

            using (var rsa = RSA.Create()) 
            using (var aes = Aes.Create())
            {
                rsa.ImportParameters(parameters);
                
                aes.IV = message.IV;
                aes.Key = rsa.Decrypt(message.Key, RSAEncryptionPadding.OaepSHA1);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                using (var decryptor = aes.CreateDecryptor())
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(message.ContentPayload);

                    return memoryStream.ToArray(); 
                }
            }
        }

        public static byte[] UnscrambledPayload(this CKCMessage message, byte[] key)
        {
            ArgumentThrow.IfNull(message, $"Invalid {typeof(CKCMessage).Name} to unscramble. Message can not be null.", nameof(message));
            ArgumentThrow.IfLengthNot(key, 16, "Invalid key to unscramble. Key buffer should be 16 bytes.", nameof(key));

            using (var aes = Aes.Create())
            {
                aes.IV = message.IV;
                aes.Key = key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                using (var decryptor = aes.CreateDecryptor())
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(message.ContentPayload);
                    
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
