using System.IO;
using System.Security.Cryptography;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    internal static class EncryptedCKCrypto 
    {
        public static byte[] UnscrambledParcel(this EncryptedCKPayload payload, byte[] key)
        {
            ArgumentThrow.IfNull(payload, $"Invalid {typeof(EncryptedCKPayload).Name} to unscramble. Payload can not be null.", nameof(payload));
            ArgumentThrow.IfLengthNot(key, 16, "Invalid key to unscramble. Key buffer should be 16 bytes.", nameof(key));

            using (var aes = Aes.Create())
            {
                // aes.IV = null;
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using (var decryptor = aes.CreateDecryptor())
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(payload.Parcel);

                    return memoryStream.ToArray(); 
                }
            }
        }
    }
}
