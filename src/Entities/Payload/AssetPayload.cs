using System;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class AssetPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] Identifier { get => this.Storage.ToArray(); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal AssetPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            
            this.Storage = buffer;
        }
    }
}
