using System;
using FoolishTech.Support.Binary;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities
{
    internal class TLLVTag
    {
        internal static class Id 
        {
            internal static TLLVTag AR { get => new TLLVTag(0x89c90f12204106b2); }
            internal static TLLVTag Asset { get => new TLLVTag(0x1bf7f53f5d5d5a1f); }
            internal static TLLVTag Capabilities { get => new TLLVTag(0x9c02af3253c07fb2); }
            internal static TLLVTag DurationCK { get => new TLLVTag(0x47acf6a418cd091a); }
            internal static TLLVTag EncryptedCK { get => new TLLVTag(0x58b38165af0e3d5a); }
            internal static TLLVTag HDCPEnforcement { get => new TLLVTag(0x2e52f1530d8ddb4a); }
            internal static TLLVTag MediaPlayback { get => new TLLVTag(0xeb8efdf2b25ab3a0); }
            internal static TLLVTag ProtocolSupported { get => new TLLVTag(0x67b8fb79ecce1a13); }
            internal static TLLVTag ProtocolUsed { get => new TLLVTag(0x5d81bcbcc7f61703); }
            internal static TLLVTag R1 { get => new TLLVTag(0xea74c4645d5efee9); }
            internal static TLLVTag R2 { get => new TLLVTag(0x71b5595ac1521133); }
            internal static TLLVTag SKR1Integrity { get => new TLLVTag(0xb349d4809e910687); }
            internal static TLLVTag SKR1 { get => new TLLVTag(0x3d1a10b8bffac2ec); }
            internal static TLLVTag StreamIndicator { get => new TLLVTag(0xabb0256a31843974); }
            internal static TLLVTag Transaction { get => new TLLVTag(0x47aa7ad3440577de); }
            internal static TLLVTag TRR { get => new TLLVTag(0x19f9d4e5ab7609cb); }
        }

        private ReadOnlyMemory<byte> Storage { get; set; }

        internal UInt64 Code { get => BinaryConverter.ReadUInt64(this.Storage.Slice(0, 8), BinaryConverter.Endianess.BigEndian); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        private TLLVTag(UInt64 code) : this(BinaryConverter.WriteUInt64(code, BinaryConverter.Endianess.BigEndian)) { }
        internal TLLVTag(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLackingBytes(buffer, 8, "Invalid buffer length. The buffer must contains enuogh bytes to fill struct.", nameof(buffer));

            this.Storage = buffer.Slice(0, 8);
        }

        internal bool Same(TLLVTag tag)
        {
            // TODO: Very inefficient comparation. (Expecially on Little Endian machines)
            return this.Code == tag.Code;
        }
    }
}