using System;
using System.Linq;
using FoolishTech.Support.Binary;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class HDCPEnforcementPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal HDCPTypeRequierement Type { get => ((HDCPTypeRequierement)BinaryConverter.ReadUInt64(this.Storage.Slice(0, 8), BinaryConverter.Endianess.BigEndian)).DefinedOrDefault(); }

        internal HDCPEnforcementPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 8, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));

            this.Storage = buffer.Slice(0, 8);
        }
    }

    internal enum HDCPTypeRequierement: UInt64
    {
        HDCPNotRequired = 0xef72894ca7895b78,

        HDCPType0Required = 0x40791ac78bd5c571,

        HDCPType1Required = 0x285a0863bba8e1d3
    }

    internal static class HDCPTypeRequierementExtensions 
    {
        public static HDCPTypeRequierement DefinedOrDefault(this HDCPTypeRequierement state)
        {   
            return Enum.IsDefined(typeof(HDCPTypeRequierement), state) ? state : HDCPTypeRequierement.HDCPType0Required;
        }
    } 
}
