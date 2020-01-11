using System;
using System.IO;
using FoolishTech.Support.Throws;
using FoolishTech.Support.Binary;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class TransactionPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal UInt64 Transaction { get => BinaryConverter.ReadUInt64(this.Storage.Slice(0, 8), BinaryConverter.Endianess.BigEndian); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal TransactionPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 8, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 8);
        }

        internal TransactionPayload(UInt64 transaction)
        {
            var stream = new MemoryStream();
            stream.Write(BinaryConverter.WriteUInt64(transaction, BinaryConverter.Endianess.BigEndian));
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());   
        }
    }
}
