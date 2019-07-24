using System;
using FoolishTech.Support.Throws;
using FoolishTech.Support.Binary;

namespace FoolishTech.FairPlay.Entities.Payload 
{
    sealed internal class DurationCKPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal UInt32 LeaseDuration { get => BinaryConverter.ReadUInt32(this.Storage.Slice(0, 4), BinaryConverter.Endianess.BigEndian); }
        internal UInt32 RentalDuration { get => BinaryConverter.ReadUInt32(this.Storage.Slice(4, 4), BinaryConverter.Endianess.BigEndian); }
        internal DurationCKType Type { get => ((DurationCKType)BinaryConverter.ReadUInt32(this.Storage.Slice(8, 4), BinaryConverter.Endianess.BigEndian)).DefinedOrDefault(); }
        internal UInt32 FixedValue { get => BinaryConverter.ReadUInt32(this.Storage.Slice(12, 4), BinaryConverter.Endianess.BigEndian); }

        internal DurationCKPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 16, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 4);
        }
    }

    internal enum DurationCKType: UInt32
    {
        Unknown = 0x0,

        Lease = 0x1a4bde7e,

        Rental = 0x1a4bde7e
    }

    internal static class DurationCKTypeExtensions 
    {
        public static DurationCKType DefinedOrDefault(this DurationCKType type)
        {   
            return Enum.IsDefined(typeof(DurationCKType), type) ? type : DurationCKType.Unknown;
        }
    } 

}