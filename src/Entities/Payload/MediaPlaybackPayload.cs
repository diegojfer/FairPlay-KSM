using System;
using System.IO;
using FoolishTech.Support.Binary;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload 
{
    sealed internal class MediaPlaybackPayload
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal DateTime CreationDate { get => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(BinaryConverter.ReadUInt32(this.Storage.Slice(0, 4), BinaryConverter.Endianess.BigEndian)); }
        internal MediaPlaybackState State { get => ((MediaPlaybackState)BinaryConverter.ReadUInt32(this.Storage.Slice(4, 4), BinaryConverter.Endianess.BigEndian)).DefinedOrDefault(); }
        internal UInt64 Session { get => BinaryConverter.ReadUInt64(this.Storage.Slice(8, 8), BinaryConverter.Endianess.BigEndian); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal MediaPlaybackPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 16, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));

            this.Storage = buffer.Slice(0, 16);
        }

        internal MediaPlaybackPayload(DateTime creationDate, MediaPlaybackState state, UInt64 session)
        {
            var stream = new MemoryStream();
            stream.Write(BinaryConverter.WriteUInt32((UInt32)(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) - creationDate).TotalSeconds, BinaryConverter.Endianess.BigEndian));
            stream.Write(BinaryConverter.WriteUInt32((UInt32)state.DefinedOrDefault(), BinaryConverter.Endianess.BigEndian));
            stream.Write(BinaryConverter.WriteUInt64(session, BinaryConverter.Endianess.BigEndian));
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());
        }

    }

    internal enum MediaPlaybackState: UInt32
    {
        Unknown = 0x00000000,

        ReadyToPlay = 0xf4dee5a2,

        Playing = 0xa5d6739e,

        PlayingExpiring = 0x4f834330
    }

    internal static class MediaPlaybackStateExtensions 
    {
        public static MediaPlaybackState DefinedOrDefault(this MediaPlaybackState state)
        {
            return Enum.IsDefined(typeof(MediaPlaybackState), state) ? state : MediaPlaybackState.Unknown;
        }
    } 
}
