using System;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class SKR1Payload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] IV { get => this.Storage.Slice(0, 16).ToArray(); }
        internal byte[] Block { get => this.Storage.Slice(16, 96).ToArray(); }

        internal SKR1Payload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 112, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 112);
        }
    }
}
