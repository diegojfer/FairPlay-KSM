using System;
using System.IO;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class SKR1IntegrityPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] Integrity { get => this.Storage.Slice(0, 16).ToArray(); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal SKR1IntegrityPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 16, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 16);
        }

        internal SKR1IntegrityPayload(byte[] integrity)
        {
            ArgumentThrow.IfLengthNot(integrity, 16, $"Invalid integrity buffer length. The buffer must contains 16 bytes.", nameof(integrity));

            var stream = new MemoryStream();
            stream.Write(integrity);
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());
        }
    }
}
