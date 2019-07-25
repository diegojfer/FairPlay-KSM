using System;
using System.Linq;
using System.Collections.Generic;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class TRRPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal IEnumerable<TLLVTag> TRR { get => Enumerable.Range(0, this.Storage.Length / 8).Select((i) => new TLLVTag(this.Storage.Slice(i * 8, 8))); }

        internal TRRPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNotMultiple(buffer, 8, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));

            this.Storage = buffer.Slice(0);
        }
    }
}
