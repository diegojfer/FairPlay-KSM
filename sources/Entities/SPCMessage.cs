using System;
using System.IO;
using FoolishTech.Support.Binary;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities
{    
    sealed internal class SPCMessage 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal UInt32 Version { get => BinaryConverter.ReadUInt32(this.Storage.Slice(0, 4), BinaryConverter.Endianess.BigEndian); }
        internal byte[] IV { get => this.Storage.Slice(8, 16).ToArray(); }
        internal byte[] Key { get => this.Storage.Slice(24, 128).ToArray(); }
        internal byte[] ProviderHash { get => this.Storage.Slice(152, 20).ToArray(); }
        internal UInt32 ContentLength { get => BinaryConverter.ReadUInt32(this.Storage.Slice(172, 4), BinaryConverter.Endianess.BigEndian); }
        internal byte[] ContentPayload { get => this.Storage.Slice(176, (int)ContentLength).ToArray(); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal SPCMessage(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLackingBytes(buffer, 176, "Invalid buffer length. The buffer must contains enuogh bytes to fill struct.", nameof(buffer));
            
            var contentLength = BinaryConverter.ReadUInt32(buffer.Slice(172, 4), BinaryConverter.Endianess.BigEndian);
            if (contentLength % 16 != 0) throw new ArgumentException("Invalid buffer. The buffer contains an unaligned payload.", nameof(buffer));
            
            var length = 0xB0 + contentLength;
            ArgumentThrow.IfLengthNot(buffer, (int)length, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));

            this.Storage = buffer.Slice(0, (int)length);
        }

        internal SPCMessage(UInt32 version, byte[] iv, byte[] key, byte[] providerHash, byte[] payload)
        {
            ArgumentThrow.IfLengthNot(iv, 16, "Invalid IV buffer length. The buffer must contains 16 bytes.", nameof(iv));
            ArgumentThrow.IfLengthNot(key, 128, "Invalid Key buffer length. The buffer must contains 128 bytes.", nameof(key));
            ArgumentThrow.IfLengthNot(providerHash, 20, "Invalid Hash buffer length. The buffer must contains 20 bytes.", nameof(providerHash));
            ArgumentThrow.IfLengthNotMultiple(payload, 16, "Invalid payload buffer length. The buffer length should be multiple of 16.", nameof(payload));

            var stream = new MemoryStream();
            stream.Write(BinaryConverter.WriteUInt32(version, BinaryConverter.Endianess.BigEndian));
            stream.Write(BinaryConverter.WriteUInt32(0, BinaryConverter.Endianess.BigEndian));
            stream.Write(iv);
            stream.Write(key);
            stream.Write(providerHash);
            stream.Write(BinaryConverter.WriteUInt32((UInt32)payload.Length, BinaryConverter.Endianess.BigEndian));
            stream.Write(payload);
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());
        }
    }
}
