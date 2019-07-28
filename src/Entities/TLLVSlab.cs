using System;
using System.Reflection;
using System.Collections.Generic;
using FoolishTech.Support.Binary;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities
{
    internal class TLLVSlab
    {
        internal static IEnumerable<TLLVSlab> FromBuffer(byte[] buffer)
        {
            ReadOnlyMemory<byte> memory = new ReadOnlyMemory<byte>(buffer);
            
            IList<TLLVSlab> slabs = new List<TLLVSlab>();
            while (memory.Length > 0) {
                var slab = new TLLVSlab(memory);
                slabs.Add(slab);
                memory = memory.Slice((int)slab.Length);
            }
            return slabs;
        }

        private ReadOnlyMemory<byte> Storage { get; set; }

        internal TLLVTag Tag { get => new TLLVTag(this.Storage.Slice(0, 8)); }
        internal UInt32 Length { get => 0x10 + BinaryConverter.ReadUInt32(this.Storage.Slice(8, 4), BinaryConverter.Endianess.BigEndian); }
        internal UInt32 ContentLength { get => BinaryConverter.ReadUInt32(this.Storage.Slice(12, 4), BinaryConverter.Endianess.BigEndian); }
        internal byte[] ContentPayload { get => this.Storage.Slice(16, (int)ContentLength).ToArray(); }
        internal T Payload<T>() => (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, new Object[] { this.Storage.Slice(16, (int)ContentLength) }, null);
      
        internal TLLVSlab(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLackingBytes(buffer, 0x10, "Invalid buffer length. The length must be the same of target struct.", nameof(buffer));

            var length = 0x10 + BinaryConverter.ReadUInt32(buffer.Slice(8, 4), BinaryConverter.Endianess.BigEndian);
            if (length % 16 != 0) throw new ArgumentException("Invalid buffer. The buffer contains an unaligned block.", nameof(buffer));
            ArgumentThrow.IfLackingBytes(buffer, (int)length, "Invalid buffer length. The buffer must contains enuogh bytes to fill struct.", nameof(buffer));

            var contentLength = BinaryConverter.ReadUInt32(buffer.Slice(12, 4), BinaryConverter.Endianess.BigEndian);
            if (contentLength > length) throw new ArgumentException("Invalid buffer. The buffer contains an invalid payload.", nameof(buffer));

            this.Storage = buffer.Slice(0, (int)length);
        }
    }
}
