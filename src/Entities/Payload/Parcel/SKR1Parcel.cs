using System;

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
    }
}
