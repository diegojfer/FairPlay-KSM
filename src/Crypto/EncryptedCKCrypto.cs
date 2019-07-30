using System.IO;
using System.Security.Cryptography;

using FoolishTech.FairPlay.Entities.Payload;
using FoolishTech.FairPlay.Entities.Payload.Parcel;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Crypto
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

        public static byte[] ScrambledParcel(this EncryptedCKParcel parcel, byte[] key)
        {
            ArgumentThrow.IfNull(parcel, $"Invalid {typeof(EncryptedCKParcel).Name} to scramble. Parcel can not be null.", nameof(parcel));
            ArgumentThrow.IfLengthNot(key, 16, "Invalid key to scramble. Key buffer should be 16 bytes.", nameof(key));

            using (var aes = Aes.Create())
            {
                // aes.IV = null;
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using (var encryptor = aes.CreateEncryptor())
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(parcel.Binary);

                    return memoryStream.ToArray(); 
                }
            }
        }
    }
}
