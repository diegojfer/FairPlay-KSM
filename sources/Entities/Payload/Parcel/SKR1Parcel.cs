using System;
using System.IO;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload.Parcel
{
    sealed internal class SKR1Parcel 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] SK { get => this.Storage.Slice(0, 16).ToArray(); }
        internal byte[] HU { get => this.Storage.Slice(16, 20).ToArray(); }
        internal byte[] R1 { get => this.Storage.Slice(36, 44).ToArray(); }
        internal byte[] Integrity { get => this.Storage.Slice(80, 16).ToArray(); }

        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal SKR1Parcel(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 96, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 96);
        }

        internal SKR1Parcel(byte[] sk, byte[] hu, byte[] r1, byte[] integrity)
        {
            ArgumentThrow.IfLengthNot(sk, 16, $"Invalid session key buffer length. The buffer must contains 16 bytes.", nameof(sk));
            ArgumentThrow.IfLengthNot(hu, 20, $"Invalid HU buffer length. The buffer must contains 20 bytes.", nameof(hu));
            ArgumentThrow.IfLengthNot(r1, 44, $"Invalid random buffer length. The buffer must contains 44 bytes.", nameof(r1));
            ArgumentThrow.IfLengthNot(integrity, 16, $"Invalid integrity buffer length. The buffer must contains 16 bytes.", nameof(integrity));

            var stream = new MemoryStream();
            stream.Write(sk);
            stream.Write(hu);
            stream.Write(r1);
            stream.Write(integrity);
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());
        }
    }
}
