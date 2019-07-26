using System;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class EncryptedCKPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] IV { get => this.Storage.Slice(0, 16).ToArray(); }
        internal byte[] Parcel { get => this.Storage.Slice(16, 16).ToArray(); }

        internal EncryptedCKPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 32, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 32);
        }
    }
}