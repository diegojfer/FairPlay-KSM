using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using FoolishTech.Support.Throws;
using FoolishTech.Support.Binary;

namespace FoolishTech.FairPlay.Crypto
{
    internal sealed class DFunction
    {
        private static UInt32 ROUNDS = 16;
        private static UInt32 PRIME = 813416437;

        private bool encryption = true;
        private byte[] ASK { get; set; }

        internal DFunction(byte[] ask, bool encryption = true)
        {
            ArgumentThrow.IfLengthNot(ask, 16, "Only 16-bytes ASKs are valid.", nameof(ask));

            this.ASK = ask;
            this.encryption = encryption;
        }

        internal byte[] DeriveBytes(byte[] bytes)
        {
            ArgumentThrow.IfLackingBytes(bytes, 1, "Invalid derivation buffer. The buffer seems to have a length lower than 1.", nameof(bytes));
            ArgumentThrow.IfTooMuchBytes(bytes, 55, "Invalid derivation buffer. The buffer seems to have a length greater than 55.", nameof(bytes));
            
            var buffer = new ReadOnlyMemory<byte>(bytes); 

            var memory = new Memory<byte>(new byte[64]);
            buffer.CopyTo(memory.Slice(0, buffer.Length));
            BinaryConverter.WriteUInt8(0x80).CopyTo(memory.Slice(buffer.Length, 1));

            UInt32 M1 = Enumerable.Range(0, 7).Select(i => BinaryConverter.ReadUInt32(memory.Slice(i * 4, 4), BinaryConverter.Endianess.BigEndian)).Aggregate((i, j) => i + j);
            for (int i = 0; i < ROUNDS; i++)
            {
                if ((M1 & 1) == 0) M1 = (3 * M1 + 1) % PRIME;
                else M1 >>= 1;
            }
            UInt32 M2 = Enumerable.Range(7, 7).Select(i => BinaryConverter.ReadUInt32(memory.Slice(i * 4, 4), BinaryConverter.Endianess.BigEndian)).Aggregate((i, j) => i + j);
            for (int i = 0; i < ROUNDS; i++)
            {
                if ((M2 & 1) == 0) M2 = (3 * M2 + 1) % PRIME;
                else M2 >>= 1;
            }

            BinaryConverter.WriteUInt32(M1, BinaryConverter.Endianess.LittleEndian).CopyTo(memory.Slice(56, 4));
            BinaryConverter.WriteUInt32(M2, BinaryConverter.Endianess.LittleEndian).CopyTo(memory.Slice(60, 4));

            using (var sha = SHA1.Create())
            using (var aes = Aes.Create())
            {
                var hash = sha.ComputeHash(memory.ToArray()).Take(16).ToArray();

                // aes.IV = new byte[16];
                aes.Key = this.ASK;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using (var encryptor = aes.CreateEncryptor())
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(hash);

                    return encryption ? memoryStream.ToArray() : hash; 
                }
            }
        }
    }
}