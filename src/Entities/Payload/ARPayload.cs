using System;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload 
{
    sealed internal class ARPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] AR { get => this.Storage.Slice(0, 16).ToArray(); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal ARPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 16, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 16);
        }
    }
}
