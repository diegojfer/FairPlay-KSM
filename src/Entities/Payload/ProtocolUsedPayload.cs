using System;
using System.IO;
using FoolishTech.Support.Throws;
using FoolishTech.Support.Binary;

namespace FoolishTech.FairPlay.Entities.Payload 
{
    sealed internal class ProtocolUsedPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal UInt32 Version { get => BinaryConverter.ReadUInt32(this.Storage.Slice(0, 4), BinaryConverter.Endianess.BigEndian); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal ProtocolUsedPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 4, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 4);
        }

        internal ProtocolUsedPayload(UInt32 version)
        {
            var stream = new MemoryStream();
            stream.Write(BinaryConverter.WriteUInt32(version, BinaryConverter.Endianess.BigEndian));
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());
        }
    }
}
