using System;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class TransactionPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] Identifier { get => this.Storage.ToArray(); }

        internal TransactionPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            
            this.Storage = buffer;
        }
    }
}
