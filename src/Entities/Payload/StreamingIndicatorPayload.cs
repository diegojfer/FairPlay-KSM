using System;
using System.IO;
using FoolishTech.Support.Binary;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class StreamingIndicatorPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal StreamingIndicator Indicator { get => ((StreamingIndicator)BinaryConverter.ReadUInt64(this.Storage.Slice(0, 8), BinaryConverter.Endianess.BigEndian)).DefinedOrDefault(); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal StreamingIndicatorPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 8, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));

            this.Storage = buffer.Slice(0, 8);
        }

        internal StreamingIndicatorPayload(StreamingIndicator indicator)
        {
            var stream = new MemoryStream();
            stream.Write(BinaryConverter.WriteUInt64((UInt64)indicator.DefinedOrDefault(), BinaryConverter.Endianess.BigEndian));
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());
        }
    }

    internal enum StreamingIndicator: UInt64
    {
        PlaybackWillOccurOnLocalDevice = 0x0,

        PlaybackWillOccurOnAVAdapter = 0x5f9c8132b59f2fde,

        PlaybackWillOccurOnAirPlay = 0xabb0256a31843974
    }

    internal static class StreamingIndicatorExtensions 
    {
        public static StreamingIndicator DefinedOrDefault(this StreamingIndicator state)
        {   
            return Enum.IsDefined(typeof(StreamingIndicator), state) ? state : StreamingIndicator.PlaybackWillOccurOnLocalDevice;
        }
    } 
}
