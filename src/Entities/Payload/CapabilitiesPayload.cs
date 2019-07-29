using System;
using System.Linq;
using FoolishTech.Support.Binary;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class CapabilitiesPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal HighCapabilities HighCapabilities { get => ((HighCapabilities)BinaryConverter.ReadUInt64(this.Storage.Slice(0, 8), BinaryConverter.Endianess.BigEndian)).DefinedOrDefault(); }
        internal LowCapabilities LowCapabilities { get => ((LowCapabilities)BinaryConverter.ReadUInt64(this.Storage.Slice(8, 8), BinaryConverter.Endianess.BigEndian)).DefinedOrDefault(); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal CapabilitiesPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 16, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));

            this.Storage = buffer.Slice(0, 16);
        }
    }

    [Flags]
    internal enum LowCapabilities: UInt64
    {
        HDCPEnforcement = 1 << 0
    }

    [Flags]
    internal enum HighCapabilities: UInt64
    {

    }

    internal static class CapabilitiesExtensions 
    {
        public static LowCapabilities DefinedOrDefault(this LowCapabilities state)
        {   
            return Enum.GetValues(typeof(LowCapabilities)).OfType<LowCapabilities>().Append((LowCapabilities)0).Append((LowCapabilities)0).Aggregate((first, second) => first | second) & state;
        }

        public static HighCapabilities DefinedOrDefault(this HighCapabilities state)
        {   
            return Enum.GetValues(typeof(HighCapabilities)).OfType<HighCapabilities>().Append((HighCapabilities)0).Append((HighCapabilities)0).Aggregate((first, second) => first | second) & state;
        }
    } 
}
